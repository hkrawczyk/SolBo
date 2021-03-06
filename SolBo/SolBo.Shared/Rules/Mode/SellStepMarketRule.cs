﻿using SolBo.Shared.Domain.Configs;
using SolBo.Shared.Domain.Enums;
using SolBo.Shared.Domain.Statics;
using SolBo.Shared.Extensions;
using SolBo.Shared.Messages.Rules;
using SolBo.Shared.Services;
using SolBo.Shared.Services.Responses;

namespace SolBo.Shared.Rules.Mode
{
    public class SellStepMarketRule : IMarketRule
    {
        public MarketOrderType MarketOrder => MarketOrderType.SELLING;
        private readonly IMarketService _marketService;
        public SellStepMarketRule(IMarketService marketService)
        {
            _marketService = marketService;
        }
        public IRuleResult RuleExecuted(Solbot solbot)
        {
            var boughtPrice = solbot.BoughtPrice();

            var result = new MarketResponse();

            if (boughtPrice > 0)
            {
                result = _marketService.IsGoodToSell(
                    solbot.Strategy.AvailableStrategy.CommissionType,
                    solbot.Strategy.AvailableStrategy.SellUp,
                    boughtPrice,
                    solbot.Communication.Price.Current);
            }
            else
            {
                result.IsReadyForMarket = false;
                result.Changed = 0;
            }

            solbot.Communication.Sell = new ChangeMessage
            {
                Change = result.Changed,
                PriceReached = result.IsReadyForMarket
            };

            var change = solbot.SellChange();

            return new MarketRuleResult()
            {
                Success = result.IsReadyForMarket,
                Message = result.Changed > 0
                    ? LogGenerator.StepMarketSuccess(MarketOrder, solbot.Communication.Price.Current, boughtPrice, change)
                    : LogGenerator.StepMarketError(MarketOrder, solbot.Communication.Price.Current, boughtPrice, change)
            };
        }
    }
}