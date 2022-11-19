using System;

namespace RdBuild.Shared.Protocol;

/// <summary>
/// Use case is following: client side is creating the command object, filling it by parameters and sends it to server by network stream serialization.
/// Server side code is registering for processor of entire command set (command enum).
/// When the server side is receiving the command, it should restore the command by its enum type.
///
/// <example>
///     the command is described by enum:
/// <code>
///     enum FooCommandSet
///     {
///         FooCommand,
///         BarCommand
///     }
/// </code>
/// So the command is creating as:
/// <code>
///     var cmd = new Command&lt;FooCommandSet&gt;(FooCommand)
/// </code>
///
/// On the server side there is the following the registration code:
///     CommandProcessor&lt;TCommand&gt; processor = new ...
///     {
///         
///     }
/// </example>
/// </summary>
/// <typeparam name="TCommandEnum"></typeparam>
class Command<TCommandEnum> where TCommandEnum : Enum
{
    private PackageContainer m_parameters;

    public Command(TCommandEnum cmd)
    {
        CommandId = cmd;
    }   

    public TCommandEnum CommandId { get; }
}

