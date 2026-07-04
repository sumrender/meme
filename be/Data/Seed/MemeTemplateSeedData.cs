using Backend.Models;

namespace Backend.Data.Seed
{
    public static class MemeTemplateSeedData
    {
        public static MemeTemplate[] Seed = new MemeTemplate[]
        {
            new()
            {
                Id = 1,
                Name = "successKid",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749139/meme_templates/wgrmqej6jktwoh8qwdt8.webp",
                Description = "A baby holding sand in his fist looking determined, representing unexpected success or minor victories.",
                Example = "Me finding a 10-dollar bill in my old winter coat."
            },
            new()
            {
                Id = 2,
                Name = "waitingSkeleton",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/bsorrrb59szpbxvlqfzh.jpg",
                Description = "A skeleton sitting on a bench, representing a long or endless wait for something.",
                Example = "Me waiting for my food delivery that I ordered 1 minute ago."
            },
            new()
            {
                Id = 3,
                Name = "grandmaFindsTheInternet",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/ohjrh7u1s5yo2k8wg2qf.jpg",
                Description = "An elderly woman looking closely at a laptop screen with a happy, naive expression.",
                Example = "Me looking for the download button on a shady torrent site."
            },
            new()
            {
                Id = 4,
                Name = "disasterGirl",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/b2eenvdnj2bwjobya5b9.jpg",
                Description = "A young girl smiling deviously at the camera while a house burns in the background.",
                Example = "When you carry the team but your team acts like they contributed as much."
            },
            new()
            {
                Id = 5,
                Name = "epicHandshake",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/dnhvaz7ifh2qbpzrvgue.jpg",
                Description = "Two muscular arms shaking hands, representing agreement or common ground between two different parties.",
                Example = "C++ developers and rust developers both hating JavaScript."
            },
            new()
            {
                Id = 6,
                Name = "laughingLeo",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749141/meme_templates/qrwnaqiosoly5o4cwmjx.png",
                Description = "Leonardo DiCaprio laughing while holding a cocktail in Django Unchained, conveying mocking amusement or sarcasm.",
                Example = "When your friend finally pays you back."
            },
            new()
            {
                Id = 7,
                Name = "monkeyLookingAway",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/mfmddhpqbba09ks3ytyi.jpg",
                Description = "A puppet monkey looking forward then turning its eyes away, representing avoiding eye contact or ignoring an obvious truth.",
                Example = "Me ignoring my compile errors and hoping they go away on the next run."
            },
            new()
            {
                Id = 8,
                Name = "leonardoDiCaprioCheers",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/esnqhfjcm5w8bnzp7g7z.jpg",
                Description = "Leonardo DiCaprio smiling and raising a glass in The Great Gatsby, expressing approval, congratulations, or cheers.",
                Example = "To the one guy on StackOverflow who answered my exact problem in 2012."
            },
            new()
            {
                Id = 9,
                Name = "whisperAndGoosebumps",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/yigvofyucfigmwc0jee7.jpg",
                Description = "A woman whispering into another woman's ear, causing an extreme reaction of goosebumps, expressing a highly exciting statement.",
                Example = "Developer: \"We are finally deleting the legacy codebase tomorrow.\""
            },
            new()
            {
                Id = 10,
                Name = "chillGuy",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/ovushouholjp0u08l9qr.jpg",
                Description = "A cartoon dog character wearing a sweater standing casually, representing a relaxed state of mind under pressure.",
                Example = "Backend server crashing while I stay completely chill."
            },
            new()
            {
                Id = 11,
                Name = "thinkingBlackGuy",
                Url = "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/eus5k9oectd3lbgozgx9.jpg",
                Description = "A man pointing to his head, indicating intelligence, smart decisions, or witty workarounds.",
                Example = "You can't have bugs in your code if you don't write any code."
            }
        };
    }
}
