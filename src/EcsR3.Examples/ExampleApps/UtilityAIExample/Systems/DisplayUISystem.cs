using System;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Components;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Types;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Plugins.UtilityAI.Extensions;
using EcsR3.Systems.Reactive;
using R3;
using Spectre.Console;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems;

public class DisplayUISystem : IReactToGroupBatchedSystem
{
    public IGroup Group => new Group(typeof(AgentComponent), typeof(CharacterDataComponent), typeof(CharacterActionComponent));
    
    public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup)
    { return Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => observableGroup); }

    public void Process(IEntityComponentAccessor entityComponentAccessor, Entity[] entities)
    {
        AnsiConsole.Clear();

        var table = new Table();
        table.AddColumn("Player").AddColumn("Health").AddColumn("Considerations").AddColumn("Advice").AddColumn("Action");

        foreach (var entity in entities)
        {
            var characterDataComponent = entityComponentAccessor.GetComponent<CharacterDataComponent>(entity);
            var agentComponent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            var characterActionComponent = entityComponentAccessor.GetComponent<CharacterActionComponent>(entity);
            var healthDisplay = $"HP: [red]{characterDataComponent.Health} / {characterDataComponent.MaxHealth}[/] \nATK: [blue]{characterDataComponent.DamagePower}[/] \nHEAL: [green]{characterDataComponent.HealPower}[/]";
            table.AddRow(new Text(characterDataComponent.Name), new Markup(healthDisplay), GetConsiderationTable(agentComponent), GetAdviceTable(agentComponent), new Markup(characterActionComponent.CurrentAction));
        }

        table.ShowRowSeparators = true;
        AnsiConsole.Write(table);
    }

    public Table GetConsiderationTable(AgentComponent agentComponent)
    {
        var table = new Table().AddColumn("Consideration").AddColumn("Score");
        foreach (var considerationId in agentComponent.ActiveConsiderations)
        {
            var considerationName = GetConsiderationName(considerationId);
            var considerationScore = agentComponent.ConsiderationVariables[considerationId];
            table.AddRow(considerationName, considerationScore.ToString("F2"));
        }

        table.ShowHeaders = false;
        return table;
    }
    
    public Table GetAdviceTable(AgentComponent agentComponent)
    {
        var bestAdviceId = agentComponent.GetBestAdviceId();
        var table = new Table().AddColumn("Advice").AddColumn("Score");
        foreach (var adviceId in agentComponent.ActiveAdvice)
        {
            var adviceName = GetAdviceName(adviceId);
            var adviceScore = agentComponent.AdviceVariables[adviceId];
            if (adviceId == bestAdviceId)
            { table.AddRow(new Markup($"[yellow]{adviceName}[/]"), new Markup($"[yellow]{adviceScore:f2}[/]")); }
            else
            { table.AddRow(adviceName, adviceScore.ToString("F2")); }
        }
        
        table.ShowHeaders = false;
        return table;
    }

    public string GetConsiderationName(int considerationId)
    {
        if(considerationId == ConsiderationTypes.Health) { return "High Health"; }
        if(considerationId == ConsiderationTypes.LowHealth) { return "Low Health"; }
        if(considerationId == ConsiderationTypes.Power) { return "Attack Power"; }
        if(considerationId == ConsiderationTypes.Healing) { return "Healing Ability"; }
        return "Unknown";
    }

    public string GetAdviceName(int adviceId)
    {
        if(adviceId == AdviceTypes.ShouldAttack) { return "Should Attack"; }
        if(adviceId == AdviceTypes.ShouldFindHealer) { return "Should Find Healer"; }
        if(adviceId == AdviceTypes.ShouldHeal) { return "Should Heal Self"; }
        return "Unknown";
    }
}