using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RavenFinance.Models
{
    public class StatementItemCreateViewModel
    {
        [Required]
        public int FinanceRecordId { get; set; }

        [Required]
        public int PaymentSourceId { get; set; }

        public int StatementItemId { get; set; }

        [Required(ErrorMessage = "Date must be current day or in the past.")]
        [DataType(DataType.Date)]
        public DateTime DateOfTransaction { get; set; }

        [Required(ErrorMessage = "Merchant name required.")]
        public string? Merchant { get; set; }

        
        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount must be a positive decimal.")]
        [Range(0.01, 10000.00)]
        public decimal Amount { get; set; }

        public bool IsExpense { get; set; }

        public bool IsDeposit { get; set; }

        public bool IsMonthlyRecurringBill { get; set; }


        //category loading options -- reason for ViewModel

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        public List<SelectListItem> CategoryOptions { get; set; } = [];
    }
}
