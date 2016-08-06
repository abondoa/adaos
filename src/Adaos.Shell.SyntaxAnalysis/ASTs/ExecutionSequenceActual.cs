using System.Collections.Generic;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// A class for the actual <see cref="ExecutionSequence"/> of the Adaos AST.
    /// The ProgramSequence is the root node of the AST, and its children are a sequence of commands.
    /// </summary>
    public class ExecutionSequenceActual : ExecutionSequence
    {
        /// <summary>
        /// Get the first command of the program sequence.
        /// </summary>
        public Execution Execution { get; private set; }

        /// <summary>
        /// Get a <see cref="ExecutionSequenceFollow"/> AST node representing the following 
        /// sequence of commands, after the first command.
        /// </summary>
        public ExecutionSequenceFollow FollowingExecutions { get; private set; }

        /// <summary>
        /// Enumerate the commands of the ProgramSequence.
        /// </summary>
        public override IEnumerable<Execution> Executions 
        {
            get 
            {
                yield return Execution;
                foreach (Execution com in FollowingExecutions.Commands)
                {
                    yield return com;
                }
            }
        }

        /// <summary>
        /// Get the position of the first character of the first command of this
        /// <see cref="ExecutionSequenceActual"/> node.
        /// </summary>
        public override int Position
        {
            get
            {
                return Execution.Position;
            }
        }

        /// <summary>
        /// A constructor for the ProgramSequenceActual
        /// </summary>
        /// <param name="command">The first command node of the ProgramSequence.</param>
        /// <param name="followingCommands">A ProgramSequenceFollow node representing a sequence of commands following the first.</param>
        public ExecutionSequenceActual(Execution command, ExecutionSequenceFollow followingCommands)
        {
            Execution = command;
            FollowingExecutions = followingCommands;
        }

        /// <summary>
        /// A constructor for the ProgramSequenceActual
        /// </summary>
        /// <param name="position">The position of the first character of this program sequence.</param>
        /// <param name="command">The first command node of the ProgramSequence.</param>
        /// <param name="followingCommands">A ProgramSequenceFollow node representing a sequence of commands following the first.</param>
        public ExecutionSequenceActual(int position, Execution command, ExecutionSequenceFollow followingCommands) : base(position)
        {
            Execution = command;
            FollowingExecutions = followingCommands;
        }


        /// <summary>
        /// The visit method used to accept an <see cref="IVisitor"/>, 
        /// for an implementation of the visitor pattern.
        /// </summary>
        /// <param name="visitor">The <see cref="IVisitor"/> to "accept".</param>
        /// <param name="obj">An object passed to the visitor.</param>
        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
