using System;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Components;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Types;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.UtilityAI.Components;
using EcsR3.Systems.Reactive;
using R3;
using Spectre.Console;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Systems;

public class DisplayUISystem : IReactToGroupBatchedSystem
{
    public IGroup Group => new Group(typeof(AgentComponent), typeof(CharacterDataComponent));
    
    public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup)
    { return Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => observableGroup); }

    public void Process(IEntityComponentAccessor entityComponentAccessor, Entity[] entities)
    {
        AnsiConsole.Clear();

        var table = new Table();
        table.AddColumn("Player").AddColumn("Health").AddColumn("Considerations").AddColumn("Advice");

        foreach (var entity in entities)
        {
            var characterDataComponent = entityComponentAccessor.GetComponent<CharacterDataComponent>(entity);
            var agentComponent = entityComponentAccessor.GetComponent<AgentComponent>(entity);
            var healthDisplay = $"[red]HP: {characterDataComponent.Health} / {characterDataComponent.MaxHealth}[/] \nATK: [blue]{characterDataComponent.DamagePower}[/] \nHEAL: [green]{characterDataComponent.HealPower}[/]";
            table.AddRow(new Text(characterDataComponent.Name), new Markup(healthDisplay), GetConsiderationTable(agentComponent), GetAdviceTable(agentComponent));
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
        var table = new Table().AddColumn("Advice").AddColumn("Score");
        foreach (var adviceId in agentComponent.ActiveAdvice)
        {
            var adviceName = GetAdviceName(adviceId);
            var adviceScore = agentComponent.AdviceVariables[adviceId];
            table.AddRow(adviceName, adviceScore.ToString("F2"));
        }
        
        table.ShowHeaders = false;
        return table;
    }

    public string GetConsiderationName(int considerationId)
    {
        if(considerationId == ConsiderationTypes.LowHealth) { return "Low Health"; }
        if(considerationId == ConsiderationTypes.Power) { return "Power"; }
        if(considerationId == ConsiderationTypes.Healing) { return "Healing"; }
        return "Unknown";
    }

    public string GetAdviceName(int adviceId)
    {
        if(adviceId == 1) { return "Heal Yourself"; }

        return "Unknown";
    }
}