﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Yaba.Common.Budget;
using Yaba.Common.Budget.DTO;

namespace Yaba.App.Models
{
	public class BudgetRepository : IBudgetRepository
	{
		private readonly HttpClient _client;
		public BudgetRepository(DelegatingHandler handler)
		{
			_client = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:50150"),
			};
		}

		public void Dispose()
		{
		}

		public async Task<BudgetDetailsDto> Find(Guid id)
		{
			var response = await _client.GetAsync($"api/budgets/{id.ToString()}");
			if (!response.IsSuccessStatusCode) throw new Exception();
			return await response.Content.To<BudgetDetailsDto>();
		}

		public async Task<ICollection<BudgetSimpleDto>> All()
		{
			var response = await _client.GetAsync("api/budgets");
			if (!response.IsSuccessStatusCode) throw new Exception();
			var budgets = await response.Content.To<ICollection<BudgetSimpleDto>>();
			return budgets;
		}

		public async Task<ICollection<BudgetSimpleDto>> AllByUser(string userId)
		{
			var response = await _client.GetAsync($"api/budgets?owner={userId}");
			if (!response.IsSuccessStatusCode) throw new Exception();
			var budgets = await response.Content.To<ICollection<BudgetSimpleDto>>();
			return budgets;
		}

		public async Task<Guid> Create(BudgetCreateUpdateDto budget)
		{
			var response = await _client.PostAsync("api/budgets", budget.ToHttpContent());
			if (response.IsSuccessStatusCode)
			{
				return Guid.Empty;
			}
			throw new Exception();
		}

		public async Task<bool> Update(BudgetCreateUpdateDto budget)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> Delete(Guid budgetId)
		{
			throw new NotImplementedException();
		}
	}
}
