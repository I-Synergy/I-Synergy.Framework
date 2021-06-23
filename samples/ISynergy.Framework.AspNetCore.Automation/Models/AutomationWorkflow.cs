using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using ISynergy.Framework.Automations;
using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.States;
using System.Linq.Expressions;
using System.Text;

namespace ISynergy.Framework.AspNetCore.Sample.Models
{
    public class AutomationWorkflow : IWorkflow<Automation>
    {
        private readonly Automation _automation;

        public string Id => _automation.AutomationId.ToString();
        public int Version => 1;

        public AutomationWorkflow(Automation automation)
        {
            _automation = automation;
        }

        public void Build(IWorkflowBuilder<Automation> builder)
        {
            if(_automation.Conditions.Count > 0)
            {
                var expression = string.Empty;
                var condition = string.Empty;

                object conditionData = null;

                var body = new StringBuilder();

                for (int i = 0; i < _automation.Conditions.Count; i++)
                {
                    var @operator = _automation.Conditions[i].Operator.ToString().ToLower();

                    foreach (var state in _automation.Conditions[i].ConditionStates)
                    {
                        switch (state.GetType().Name)
                        {
                            case nameof(EventState):
                                if (state is EventState eventState)
                                {
                                    body.AppendLine($"({eventState.Event} == {condition} && {eventState.EventData} == {conditionData}) {@operator}");
                                }
                                break;
                            case nameof(TimeState):
                                if (state is TimeState timeState && TimeSpan.TryParse(condition, out TimeSpan time))
                                {
                                    body.AppendLine($"({time} >= {timeState.After} && {time} <= {timeState.Before}) {@operator}");
                                }
                                break;
                            case nameof(IntegerState):
                                if (state is IntegerState intState && int.TryParse(condition, out int intValue))
                                {
                                    body.AppendLine($"({intValue} >= {intState.From} && {intValue} <= {intState.To}) {@operator}");
                                }
                                break;
                            case nameof(DecimalState):
                                if (state is DecimalState decimalState && decimal.TryParse(condition, out decimal decValue))
                                {
                                    body.AppendLine($"({decValue} >= {decimalState.From} && {decValue} <= {decimalState.To}) {@operator}");
                                }
                                break;
                            case nameof(DoubleState):
                                if (state is IntegerState doubleState && double.TryParse(condition, out double dblValue))
                                {
                                    body.AppendLine($"({dblValue} >= {doubleState.From} && {dblValue} <= {doubleState.To}) {@operator}");
                                }
                                break;
                            case nameof(BooleanState):
                                if (state is BooleanState booleanState && bool.TryParse(condition, out bool boolValue))
                                {
                                    body.AppendLine($"({boolValue} == {booleanState}) {@operator}");
                                }
                                break;
                            default:
                                if(state is StringState stringState)
                                {
                                    body.AppendLine($"({condition} == {stringState}) {@operator}");
                                }
                                break;
                        }
                    }

                    
                }
            }


            throw new NotImplementedException();
        }
    }
}
