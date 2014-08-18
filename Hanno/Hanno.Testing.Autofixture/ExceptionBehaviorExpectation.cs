using System;
using Ploeh.AutoFixture.Idioms;

namespace Hanno.Testing.Autofixture
{
    public class ExceptionBehaviorExpectation<T> : IBehaviorExpectation where T : Exception
    {
        /// <summary>
        /// Verifies that the command behaves correct when invoked with a null argument.
        /// </summary>
        /// <param name="command">The command whose behavior must be examined.</param>
        /// <remarks>
        /// <para>
        /// The Verify method attempts to invoke the <paramref name="command" /> instance's
        /// <see cref="IGuardClauseCommand.Execute" /> with <see langword="null" />. The expected
        /// result is that this action throws an <see cref="ArgumentNullException" />, in which
        /// case the expected behavior is considered verified. If any other exception is thrown, or
        /// if no exception is thrown at all, the verification fails and an exception is thrown.
        /// </para>
        /// <para>
        /// The behavior is only asserted if the command's
        /// <see cref="IGuardClauseCommand.RequestedType" /> is nullable. In case of value types,
        /// no action is performed.
        /// </para>
        /// </remarks>
        public void Verify(IGuardClauseCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");


            if (!command.RequestedType.IsClass
                && !command.RequestedType.IsInterface)
            {
                return;
            }


            try
            {
                command.Execute(null);
            }
            catch (T)
            {
                return;
            }
            catch (Exception e)
            {
                throw command.CreateException("null", e);
            }


            throw command.CreateException("null");
        }
    }

}