using Backend.Data.UnitOfWork;
using Backend.Dtos;
using Backend.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Backend.Services
{
    public class MemeService : IMemeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAiService _aiService;
        private readonly IMemoryCache _cache;
        private Dictionary<string, string> _memeNameToImageDict = new();

        public MemeService(IUnitOfWork unitOfWork, IAiService aiService, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _aiService = aiService;
            _cache = cache;
        }

        private void SetMemeNameToImageDict(IEnumerable<MemeTemplate> memes)
        {
            _memeNameToImageDict.Clear();
            foreach (var meme in memes)
            {
                _memeNameToImageDict.Add(meme.Name, meme.Url);
            }
        }

        private async Task<IEnumerable<MemeTemplate>> GetMemes()
        {
            if (_cache.TryGetValue("meme_templates", out IEnumerable<MemeTemplate>? cached) && cached != null)
            {
                SetMemeNameToImageDict(cached);
                return cached;
            }

            var memes = await _unitOfWork.MemeTemplates.GetAllAsync();
            SetMemeNameToImageDict(memes);
            _cache.Set("meme_templates", memes, TimeSpan.FromHours(1));
            return memes;
        }

        private string ListMemesForPrompt(IEnumerable<MemeTemplate> memes)
        {
            string memesDescription = "";
            foreach (var meme in memes)
            {
                string description = $"{meme.Name}: {meme.Description} | {meme.Example} \n";
                memesDescription += description;
            }
            return memesDescription;
        }

        private Dictionary<string, string> ParseJsonToDictionary(string jsonData)
        {
            jsonData = jsonData.Trim();
            if (jsonData.StartsWith("```json"))
            {
                jsonData = jsonData.Substring(7);
            }
            if (jsonData.EndsWith("```"))
            {
                jsonData = jsonData.Substring(0, jsonData.Length - 3);
            }

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData)!;
        }

        public async Task<List<MemeResponseDto>> GetRandomMemes()
        {
            var memes = await GetMemes();
            string prompt = @"
                    Given a list of memes with their descriptions and associated emotions, your goal as a very funny and high performing LLM
                    is to generate hilarious captions that can be used with the memes

                    Example 
                    Input=> 
                    
                    waitingSkeleton: A skeleton sitting on a bench, often used to humorously represent a long or endless wait for something. | Example: Me waiting for my food delivery that I ordered 1 minute ago 
                    laughingLeo: Leonardo DiCaprio laughing while holding a cocktail in the movie Django Unchained, often used to convey mocking amusement or sarcasm. | Example: When your friend finally pays you back.

                    OUTPUT => {
                        'waitingSkeleton': 'Me waiting at the grocery store for my mom to stop talking to her friend so I can go home.',
                        'laughingLeo': 'The FBI checks my phone but has to go through 60,000 memes'
                    }

                    Like above examples, generate hilarious funny captions for all the memes.
                    Please take reference from the Examples that each meme have and the captions that you generate should at least be that much funny otherwise my friends will make fun of me and I'll be sad.
                    Input=> " + this.ListMemesForPrompt(memes) + @" 
                    OUTPUT (The output should only contain serialized JSON data, and should not start with ```json) =>";
            var captions = await _aiService.AskAsync(prompt);
            var memesArray = new List<MemeResponseDto>();

            var captionsDictionary = ParseJsonToDictionary(captions);
            foreach (var pair in captionsDictionary)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
            if (captionsDictionary != null)
            {
                foreach (var kvp in captionsDictionary)
                {
                    if (_memeNameToImageDict.ContainsKey(kvp.Key))
                    {
                        var memeTemplate = _memeNameToImageDict[kvp.Key];
                        var caption = kvp.Value;
                        var meme = new MemeResponseDto
                        {
                            Caption = caption,
                            MemeTemplate = memeTemplate
                        };
                        memesArray.Add(meme);
                    }
                    else
                    {
                        Console.WriteLine($"Meme template for '{kvp.Key}' not found.");
                    }
                }
            }
            return memesArray;
        }

        public async Task<List<MemeResponseDto>> GenerateMemesForContent(string content)
        {
            var memes = await GetMemes();
            string prompt = @"
            You are given a list of meme templates with their names and description. You will also be given some text content.
            Your goal as a very funny and high performing LLM is to go through the text content and find interesting areas, and
            then using or referencing those interesting bits to create a list of funny captions for all the meme template below.
            Here is the list of meme templates:
            " + this.ListMemesForPrompt(memes) + @"     
            Example:
            INPUT => ```
                Dear Diary,
                Today was our group presentation, and I'm exhausted. I did most of the work—researching, creating slides, and practicing—while my groupmates barely contributed. The presentation went well, but watching them share the applause felt frustrating.
                To top it off, another group presented an idea suspiciously similar to ours. Seeing the professor praise their ""unique perspective"" was infuriating.
                I guess life isn't fair, but I've learned a lesson about teamwork and standing up for myself. For now, I'm just relieved it's over.
                Good night.
                -John
            ```

            OUTPUT => {
                'disasterGirl': 'When you carry the team but your team acts like they contributed as much.',
                'spidermanPointing': 'Us and the other group during presentations: \""No, it was our idea!\""'
            }

            VERY IMPORTANT - DON'T USE ANY OF THESE EXAMPLES IN THE OUTPUT. USE THEM ONLY TO UNDERSTAND HOW TO GENERATE MEME CAPTIONS.

            Seeing the above example I want you to generate a json object with meme captions from the above memes list for the following
            INPUT => " + content + @"
                    OUTPUT (The output should only contain serialized JSON data, and should not start with ```json) => 
                    ";
            var captions = await _aiService.AskAsync(prompt);
            var memesArray = new List<MemeResponseDto>();

            var captionsDictionary = ParseJsonToDictionary(captions);
            foreach (var pair in captionsDictionary)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
            if (captionsDictionary != null)
            {
                foreach (var kvp in captionsDictionary)
                {
                    if (_memeNameToImageDict.ContainsKey(kvp.Key))
                    {
                        var memeTemplate = _memeNameToImageDict[kvp.Key];
                        var caption = kvp.Value;
                        var meme = new MemeResponseDto
                        {
                            Caption = caption,
                            MemeTemplate = memeTemplate
                        };
                        memesArray.Add(meme);
                    }
                    else
                    {
                        Console.WriteLine($"Meme template for '{kvp.Key}' not found.");
                    }
                }
            }
            return memesArray;
        }
    }
}
