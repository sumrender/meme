using Backend.Data.UnitOfWork;
using Backend.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Services
{
    public interface ICreditService
    {
        Task<int> GetTotalCreditsAsync(int userId);
        Task<(bool Success, int RemainingCredits)> TryDeductCreditAsync(int userId);
        Task RefundCreditAsync(int userId);
        Task GrantInitialCreditsAsync(int userId);
    }

    public class CreditService : ICreditService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreditService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetTotalCreditsAsync(int userId)
        {
            var credits = await _unitOfWork.Credits.GetByUserIdAsync(userId);
            return credits.Sum(c => c.Amount);
        }

        public async Task<(bool Success, int RemainingCredits)> TryDeductCreditAsync(int userId)
        {
            IDbContextTransaction? transaction = null;
            try
            {
                transaction = await _unitOfWork.BeginTransactionAsync();

                var credits = (await _unitOfWork.Credits.GetByUserIdForUpdateAsync(userId)).ToList();
                if (credits.Count == 0)
                {
                    await transaction.RollbackAsync();
                    return (false, 0);
                }

                var freeCredit = credits.FirstOrDefault(c => c.CreditType == CreditType.FREE && c.Amount > 0);
                var paidCredit = credits.FirstOrDefault(c => c.CreditType == CreditType.PAID && c.Amount > 0);

                UserCredit? deducted = null;

                if (freeCredit != null)
                {
                    freeCredit.Amount -= 1;
                    freeCredit.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Credits.Update(freeCredit);
                    deducted = freeCredit;
                }
                else if (paidCredit != null)
                {
                    paidCredit.Amount -= 1;
                    paidCredit.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Credits.Update(paidCredit);
                    deducted = paidCredit;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return (false, 0);
                }

                await _unitOfWork.CreditTransactions.AddAsync(new CreditTransaction
                {
                    UserId = userId,
                    CreditType = deducted!.CreditType,
                    TransactionType = TransactionType.DEDUCTION,
                    Amount = -1,
                    ReferenceType = "ALBUM_GENERATION",
                    CreatedAt = DateTime.UtcNow
                });

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                var remaining = credits.Sum(c => c.Amount);
                return (true, remaining);
            }
            catch
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw;
            }
        }

        public async Task RefundCreditAsync(int userId)
        {
            IDbContextTransaction? transaction = null;
            try
            {
                transaction = await _unitOfWork.BeginTransactionAsync();

                var credits = (await _unitOfWork.Credits.GetByUserIdForUpdateAsync(userId)).ToList();
                var freeCredit = credits.FirstOrDefault(c => c.CreditType == CreditType.FREE);
                var paidCredit = credits.FirstOrDefault(c => c.CreditType == CreditType.PAID);

                var target = freeCredit ?? paidCredit;
                if (target == null)
                {
                    await transaction.RollbackAsync();
                    return;
                }

                target.Amount += 1;
                target.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Credits.Update(target);

                await _unitOfWork.CreditTransactions.AddAsync(new CreditTransaction
                {
                    UserId = userId,
                    CreditType = target.CreditType,
                    TransactionType = TransactionType.REFUND,
                    Amount = 1,
                    ReferenceType = "GENERATION_FAILURE_REFUND",
                    CreatedAt = DateTime.UtcNow
                });

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw;
            }
        }

        public async Task GrantInitialCreditsAsync(int userId)
        {
            var existing = await _unitOfWork.Credits.GetByUserIdAsync(userId);
            if (existing.Any(c => c.CreditType == CreditType.FREE))
            {
                return;
            }

            await _unitOfWork.Credits.AddAsync(new UserCredit
            {
                UserId = userId,
                CreditType = CreditType.FREE,
                Amount = 5,
                UpdatedAt = DateTime.UtcNow
            });

            await _unitOfWork.CreditTransactions.AddAsync(new CreditTransaction
            {
                UserId = userId,
                CreditType = CreditType.FREE,
                TransactionType = TransactionType.GRANT,
                Amount = 5,
                ReferenceType = "SIGNUP_BONUS",
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
