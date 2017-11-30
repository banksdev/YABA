using System;
using Moq;
using Xunit;
using Yaba.Common.Budget.DTO;
using Yaba.Entities.Budget;

namespace Yaba.Entities.Test.Budget
{
	public class EFBudgetRepositoryTests
	{


		[Fact (DisplayName = "Using repository disposes the context - Budget")]
		public void Using_repository_disposes_of_context()
		{
			var mock = new Mock<IYabaDBContext>();
			using (var repo = new EFBudgetRepository(mock.Object));
			mock.Verify(m => m.Dispose(), Times.Once);
		}

		[Fact(DisplayName = "FindAllBudgets returns a collection of budgets")]
		public async void FindAllBudgets_returns_collection_of_budgets()
		{
			var context = Util.GetNewContext(nameof(FindAllBudgets_returns_collection_of_budgets));

			var budget1 = new Entities.Budget.BudgetEntity {Name = "First"};
			var budget2 = new Entities.Budget.BudgetEntity {Name = "Second"};
			var budget3 = new Entities.Budget.BudgetEntity {Name = "Third"};

			using (var repo = new EFBudgetRepository(context))
			{
				context.Budgets.AddRange(budget1, budget2, budget3);
				await context.SaveChangesAsync();

				var budgets = await repo.All();
				Assert.Equal(3, budgets.Count);
			}
		}

		[Fact(DisplayName = "FindBudget with existing id returns a budget")]
		public async void FindBudget_given_existing_id_returns_budget()
		{
			var context = Util.GetNewContext(nameof(FindBudget_given_existing_id_returns_budget));

			var budget = new Entities.Budget.BudgetEntity {Name = "New Budget"};

			using (var repo = new EFBudgetRepository(context))
			{
				context.Budgets.Add(budget);
				await context.SaveChangesAsync();
				var budgetDTO = await repo.Find(budget.Id);
				Assert.Equal("New Budget", budgetDTO.Name);
			}
		}

		[Fact(DisplayName = "FindBudget given non-existing id returns null")]
		public async void FindBudget_given_nonexisting_id_returns_null()
		{
			var context = Util.GetNewContext(nameof(FindBudget_given_nonexisting_id_returns_null));

			using (var repo = new EFBudgetRepository(context))
			{
				var budgetDTO = await repo.Find(Guid.Empty);
				Assert.Null(budgetDTO);
			}
		}

		[Fact(DisplayName = "CreateBudget creates a budget")]
		public async void CreateBudget_creates_budgets()
		{
			var entity = default(Entities.Budget.BudgetEntity);
			var mock = new Mock<IYabaDBContext>();

			using (var repo = new EFBudgetRepository(mock.Object))
			{
				mock.Setup(m => m.Budgets.Add(It.IsAny<Entities.Budget.BudgetEntity>()))
				.Callback<Entities.Budget.BudgetEntity>(t => entity = t);
				var bcdto = new BudgetCreateUpdateDto {Name = "My Budget"};
				await repo.Create(bcdto);
			}

			Assert.Equal("My Budget", entity.Name);
		}

		[Fact (DisplayName = "UpdateBudget updates an existing budget")]
		public async void UpdateBudget_updates_existing_budget()
		{
			var context = Util.GetNewContext(nameof(UpdateBudget_updates_existing_budget));
			var budget = new Entities.Budget.BudgetEntity
			{
				Name = "Not updated"
			};
			context.Budgets.Add(budget);
			await context.SaveChangesAsync();

			using (var repo = new EFBudgetRepository(context))
			{
				var updatedBudget = new BudgetCreateUpdateDto
				{
					Id = budget.Id,
					Name = "Updated",
				};
				var updated = await repo.Update(updatedBudget);
				Assert.True(updated);
				Assert.Equal("Updated", budget.Name);
			}
		}

		[Fact (DisplayName = "UpdateBudget given DTO with no ID returns false")]
		public async void UpdateBudget_given_dto_with_no_id_returns_false()
		{
			var context = Util.GetNewContext(nameof(UpdateBudget_given_dto_with_no_id_returns_false));
			using (var repo = new EFBudgetRepository(context))
			{
				var updated = await repo.Update(new BudgetCreateUpdateDto());
				Assert.False(updated);
			}
		}

		[Fact (DisplayName = "UpdateBudget given DTO with non-existing id returns false")]
		public async void UpdateBudget_given_dto_with_nonexisting_id_returns_false()
		{
			var context = Util.GetNewContext(nameof(UpdateBudget_given_dto_with_nonexisting_id_returns_false));
			using (var repo = new EFBudgetRepository(context))
			{
				var dto = new BudgetCreateUpdateDto
				{
					Id = Guid.NewGuid(),
				};
				var updated = await repo.Update(dto);
				Assert.False(updated);
			}
		}

		//[Fact]
		//public async void AddEntry_given_valid_entry_returns_true()
		//{
		//    var result = false;
		//    var budget = new Budget { Name = "hello"};
		//    var categories = new BudgetCategory {  Name="helloCategory", Budget = budget};
		//    budget.Categories = new List<BudgetCategory>{ categories };

		//    var context = Util.GetNewContext(nameof(AddEntry_given_valid_entry_returns_true));
		//    using(var repo = new EFBudgetRepository(context))
		//    {
		//        context.Budgets.Add(budget);
		//        context.BudgetCategories.Add(categories);

		//        var entry = new BudgetEntryDTO
		//        {
		//            Amount = 10,
		//            Date = DateTime.Now,
		//            Description = "",
		//            BudgetCategory = new BudgetCategoryDTO
		//            {
		//                Id = categories.Id
		//            }
		//        };
		//        result = await repo.(entry);
		//    }
		//    Assert.True(result);
		//}

		//[Fact]
		//public async void AddEntry_given_entry_with_no_budgetCategory_returns_false()
		//{
		//    var result = false;
		//    var budget = new Budget { Name = "hello" };
		//    var categories = new BudgetCategory { Name = "helloCategory", Budget = budget };
		//    budget.Categories = new List<BudgetCategory> { categories };

		//    var context = Util.GetNewContext(nameof(AddEntry_given_entry_with_no_budgetCategory_returns_false));
		//    using (var repo = new EFBudgetRepository(context))
		//    {
		//        context.Budgets.Add(budget);
		//        context.BudgetCategories.Add(categories);

		//        var entry = new BudgetEntryDTO
		//        {
		//            Amount = 10,
		//            Date = DateTime.Now,
		//            Description = ""
		//        };
		//        result = await repo.AddEntryToCategory(entry);
		//    }
		//    Assert.False(result);
		//}

		//[Fact]
		//public async void AddEntry_given_default_entry_returns_false()
		//{
		//    var result = false;
		//    var context = Util.GetNewContext(nameof(AddEntry_given_default_entry_returns_false));
		//    using (var repo = new EFBudgetRepository(context))
		//    {
		//        var entry = default(BudgetEntryDTO);

		//        result = await repo.AddEntryToCategory(entry);
		//    }

		//    Assert.False(result);
		//}
	}
}
