namespace Chickensoft.LogicBlocks.Tests.Fixtures;

using Chickensoft.LogicBlocks.Generator;

[StateMachine]
public partial class ErrorLogic : LogicBlock<ErrorLogic.State> {
  public override State GetInitialState(IContext context) =>
    new State.StateA(context);

  public static class Input {
    public readonly record struct GoToB(bool ShouldThrow);
    public readonly record struct GoToA;
  }

  public abstract record State : StateLogic {
    public List<string> Updates { get; init; } = new();

    public State(IContext context) : base(context) {
      OnEnter<State>(
        (_) => Updates.Add("Enter " + nameof(State))
      );
      OnExit<State>(
        (_) => Updates.Add("Exit " + nameof(State))
      );
    }

    public record StateA : State, IGet<Input.GoToB> {
      public StateA(IContext context) : base(context) {
        OnEnter<StateA>(
          (_) => Updates.Add("Enter " + nameof(StateA))
        );
        OnExit<StateA>(
          (_) => Updates.Add("Exit " + nameof(StateA))
        );
      }

      public State On(Input.GoToB input) {
        if (input.ShouldThrow) {
          throw new InvalidOperationException("Error");
        }

        return new StateB(Context) { Updates = Updates };
      }
    }

    public record StateB : State, IGet<Input.GoToA> {
      public StateB(IContext context) : base(context) {
        OnEnter<StateB>(
          (_) => {
            Updates.Add("Enter " + nameof(StateB));
            throw new InvalidOperationException("Enter Error");
          }
        );
        OnExit<StateB>(
          (_) => Updates.Add("Exit " + nameof(StateB))
        );
      }

      public State On(Input.GoToA input) => new StateA(Context) {
        Updates = Updates
      };
    }
  }
}
