﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Yaba.Common.DTOs.Category;
using Yaba.Entities.BudgetEntities;

namespace Yaba.Entities.Test
{
    public class EFCategoryRepositoryTests
    {
        
        /**
         * Interface methods:
         *
         * √ Task<ICollection<CategorySimpleDto>> Find();
         * √ Task<CategoryDetailsDto> Find(Guid id);
         * √ Task<Guid?> Create(CategoryCreateDto category);
         * √ Task<bool> Update(CategorySimpleDto category);
         * - Task<bool> Delete(Guid id);
         * 
         */

        [Fact]
        public async void Find_given_no_arguments_returns_collection_of_budgets()
        {
            var ctx = Util.GetNewContext(nameof(Find_given_no_arguments_returns_collection_of_budgets));
            ctx.BudgetCategories.AddRange(new []
            {
                new BudgetCategory(), 
                new BudgetCategory(), 
                new BudgetCategory(), 
            });
            await ctx.SaveChangesAsync();

            using (var repo = new EFCategoryRepository(ctx))
            {
                var categories = await repo.Find();
                Assert.Equal(3, categories.Count);
            }
        }

        [Fact]
        public async void Find_given_existing_id_returns_category()
        {
            var ctx = Util.GetNewContext(nameof(Find_given_existing_id_returns_category));
            var entity = new BudgetCategory
            {
                Name = "Find Category"
            };
            ctx.BudgetCategories.Add(entity);
            await ctx.SaveChangesAsync();

            using (var repo = new EFCategoryRepository(ctx))
            {
                var category = await repo.Find(entity.Id);
                Assert.Equal("Find Category", category.Name);
            }
        }

        [Fact]
        public async void Find_given_nonexisting_id_returns_null()
        {
            var ctx = Util.GetNewContext(nameof(Find_given_nonexisting_id_returns_null));
            using (var repo = new EFCategoryRepository(ctx))
            {
                var category = await repo.Find(Guid.NewGuid());
                Assert.Null(category);
            }
        }

        [Fact]
        public async void FindFromBudget_given_existing_budget_id_returns_list_of_categories()
        {
            var ctx = Util.GetNewContext(nameof(FindFromBudget_given_existing_budget_id_returns_list_of_categories));
            var budget = new Budget();
            ctx.Budgets.Add(budget);
            await ctx.SaveChangesAsync();
            ctx.BudgetCategories.AddRange(new []
            {
                new BudgetCategory { Budget = budget },
                new BudgetCategory { Budget = budget },
            });
            await ctx.SaveChangesAsync();
            
            using (var repo = new EFCategoryRepository(ctx))
            {
                var cats = await repo.FindFromBudget(budget.Id);
                Assert.Equal(2, cats.Count);
            }
        }

        [Fact]
        public async void FindFromBudget_given_nonexisting_budget_id_returns_empty_list()
        {
            var ctx = Util.GetNewContext(nameof(FindFromBudget_given_nonexisting_budget_id_returns_empty_list));
            using (var repo = new EFCategoryRepository(ctx))
            {
                var cats = await repo.FindFromBudget(Guid.NewGuid());
                Assert.Empty(cats);
            }
        }
        
        

        [Fact]
        public async void Create_creates_new_category()
        {
            var ctx = Util.GetNewContext(nameof(Create_creates_new_category));
            using (var repo = new EFCategoryRepository(ctx))
            {
                var cat = new CategoryCreateDto
                {
                    Name = "Create Category",
                };
                var guid = await repo.Create(cat);
                var entity = await ctx.BudgetCategories
                    .SingleOrDefaultAsync(c => c.Id == guid);
                
                Assert.Equal("Create Category", entity.Name);
            }
        }

        [Fact]
        public async void Update_given_existing_id_updates_entity()
        {
            var ctx = Util.GetNewContext(nameof(Update_given_existing_id_updates_entity));
            var entity = new BudgetCategory
            {
                Name = "Not Updated"
            };
            ctx.BudgetCategories.Add(entity);
            await ctx.SaveChangesAsync();

            using (var repo = new EFCategoryRepository(ctx))
            {
                var update = new CategorySimpleDto
                {
                    Id = entity.Id,
                    Name = "Updated",
                };
                var updated = await repo.Update(update);

                Assert.True(updated);
                Assert.Equal("Updated", entity.Name);
            }
        }

        [Fact]
        public async void Update_given_nonexisting_id_returns_false()
        {
            var ctx = Util.GetNewContext(nameof(Update_given_nonexisting_id_returns_false));
            using (var repo = new EFCategoryRepository(ctx))
            {
                var updated = await repo.Update(new CategorySimpleDto());
                Assert.False(updated);
            }
        }

        [Fact]
        public async void Delete_given_existing_id_removes_entity()
        {
            var ctx = Util.GetNewContext(nameof(Delete_given_existing_id_removes_entity));
            var entity = new BudgetCategory
            {
                Name = "Delete Category",
            };
            ctx.BudgetCategories.Add(entity);
            await ctx.SaveChangesAsync();

            using (var repo = new EFCategoryRepository(ctx))
            {
                var deleted = await repo.Delete(entity.Id);

                Assert.True(deleted);
                var cat = await ctx.BudgetCategories
                    .SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.Null(cat);
            }
        }

        [Fact]
        public async void Delete_given_nonexisting_id_returns_false()
        {
            var ctx = Util.GetNewContext(nameof(Delete_given_existing_id_removes_entity));
            using (var repo = new EFCategoryRepository(ctx))
            {
                var deleted = await repo.Delete(Guid.NewGuid());
                Assert.False(deleted);
            }
        }

    }
}