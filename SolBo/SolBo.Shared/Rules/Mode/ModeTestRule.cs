﻿using NLog;
using SolBo.Shared.Domain.Configs;
using SolBo.Shared.Domain.Statics;
using SolBo.Shared.Rules.Mode.Test;
using SolBo.Shared.Services;
using System.Collections.Generic;

namespace SolBo.Shared.Rules.Mode
{
    public class ModeTestRule : IModeRule
    {
        public string ModeName => "TEST MODE";
        private static readonly Logger Logger = LogManager.GetLogger("SOLBO");
        private readonly IMarketService _marketService;
        private readonly ICollection<IMarketRule> _rules = new HashSet<IMarketRule>();
        public ModeTestRule(IMarketService marketService)
        {
            _marketService = marketService;
        }
        public IRuleResult RuleExecuted(Solbot solbot)
        {
            _rules.Add(new StopLossStepMarketRule(_marketService));
            _rules.Add(new StopLossExecuteMarketTestRule());

            _rules.Add(new SellStepMarketRule(_marketService));
            _rules.Add(new SellExecuteMarketTestRule());

            _rules.Add(new BuyStepMarketRule(_marketService));
            _rules.Add(new BuyExecuteMarketTestRule());

            Logger.Info(LogGenerator.ModeStart(ModeName));

            foreach (var item in _rules)
            {
                var result = item.RuleExecuted(solbot);

                Logger.Info($"{result.Message}");
            }

            Logger.Info(LogGenerator.ModeEnd(ModeName));

            return new ModeRuleResult
            {
                Message = LogGenerator.ModeExecuted(ModeName),
                Success = true
            };
        }
    }
}