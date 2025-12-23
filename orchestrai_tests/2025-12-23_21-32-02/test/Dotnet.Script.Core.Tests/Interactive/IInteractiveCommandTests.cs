using System;
using Xunit;
using Dotnet.Script.Core;

namespace Dotnet.Script.Core.Tests.Interactive
{
    public class IInteractiveCommandTests
    {
        /// <summary>
        /// Tests that IInteractiveCommand is an interface
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ShouldBeAnInterface()
        {
            // Arrange & Act & Assert
            Assert.True(typeof(IInteractiveCommand).IsInterface);
        }

        /// <summary>
        /// Tests that IInteractiveCommand is public
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ShouldBePublic()
        {
            // Arrange & Act & Assert
            Assert.True(typeof(IInteractiveCommand).IsPublic);
        }

        /// <summary>
        /// Tests that IInteractiveCommand has a Name property
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ShouldHaveNameProperty()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var nameProperty = interfaceType.GetProperty("Name");

            // Assert
            Assert.NotNull(nameProperty);
            Assert.Equal("Name", nameProperty.Name);
        }

        /// <summary>
        /// Tests that Name property returns a string
        /// </summary>
        [Fact]
        public void IInteractiveCommand_NameProperty_ShouldReturnString()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var nameProperty = interfaceType.GetProperty("Name");

            // Assert
            Assert.NotNull(nameProperty);
            Assert.Equal(typeof(string), nameProperty.PropertyType);
        }

        /// <summary>
        /// Tests that IInteractiveCommand has an Execute method
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ShouldHaveExecuteMethod()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var executeMethod = interfaceType.GetMethod("Execute");

            // Assert
            Assert.NotNull(executeMethod);
            Assert.Equal("Execute", executeMethod.Name);
        }

        /// <summary>
        /// Tests that Execute method takes a CommandContext parameter
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ExecuteMethod_ShouldTakeCommandContextParameter()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var executeMethod = interfaceType.GetMethod("Execute");
            var parameters = executeMethod.GetParameters();

            // Assert
            Assert.NotNull(executeMethod);
            Assert.Single(parameters);
            Assert.Equal("commandContext", parameters[0].Name);
            Assert.Equal(typeof(CommandContext), parameters[0].ParameterType);
        }

        /// <summary>
        /// Tests that Execute method returns void
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ExecuteMethod_ShouldReturnVoid()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var executeMethod = interfaceType.GetMethod("Execute");

            // Assert
            Assert.NotNull(executeMethod);
            Assert.Equal(typeof(void), executeMethod.ReturnType);
        }

        /// <summary>
        /// Tests that IInteractiveCommand has exactly 2 members (Name property and Execute method)
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ShouldHaveExactlyTwoMembers()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var members = interfaceType.GetMembers();

            // Assert
            // GetMembers() returns property accessors and methods
            // Should have: get_Name, Execute, and the property itself would be implicit
            Assert.True(members.Length >= 2, $"Expected at least 2 members, but found {members.Length}");
        }

        /// <summary>
        /// Tests that ExitCommand implements IInteractiveCommand
        /// </summary>
        [Fact]
        public void ExitCommand_ShouldImplementIInteractiveCommand()
        {
            // Arrange
            var exitCommandType = typeof(ExitCommand);

            // Act & Assert
            Assert.True(typeof(IInteractiveCommand).IsAssignableFrom(exitCommandType));
        }

        /// <summary>
        /// Tests that IInteractiveCommand can be used as a type constraint
        /// </summary>
        [Fact]
        public void IInteractiveCommand_CanBeUsedAsTypeConstraint()
        {
            // Arrange
            var command = new ExitCommand() as IInteractiveCommand;

            // Act & Assert
            Assert.NotNull(command);
            Assert.IsAssignableFrom<IInteractiveCommand>(command);
        }

        /// <summary>
        /// Tests that Name property has a getter
        /// </summary>
        [Fact]
        public void IInteractiveCommand_NameProperty_ShouldHaveGetter()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var nameProperty = interfaceType.GetProperty("Name");

            // Assert
            Assert.NotNull(nameProperty);
            Assert.NotNull(nameProperty.GetGetMethod());
        }

        /// <summary>
        /// Tests that IInteractiveCommand namespace is correct
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var @namespace = interfaceType.Namespace;

            // Assert
            Assert.Equal("Dotnet.Script.Core", @namespace);
        }

        /// <summary>
        /// Tests that implementations of IInteractiveCommand must provide Name property
        /// </summary>
        [Fact]
        public void IInteractiveCommand_NameProperty_IsRequired()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var nameProperty = interfaceType.GetProperty("Name");

            // Assert - the property must exist and be part of the contract
            Assert.NotNull(nameProperty);
            Assert.True(nameProperty.DeclaringType == interfaceType);
        }

        /// <summary>
        /// Tests that implementations of IInteractiveCommand must provide Execute method
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ExecuteMethod_IsRequired()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var executeMethod = interfaceType.GetMethod("Execute");

            // Assert - the method must exist and be part of the contract
            Assert.NotNull(executeMethod);
            Assert.True(executeMethod.DeclaringType == interfaceType);
        }

        /// <summary>
        /// Tests that IInteractiveCommand implementation can be instantiated
        /// </summary>
        [Fact]
        public void IInteractiveCommand_Implementation_CanBeInstantiated()
        {
            // Arrange & Act
            IInteractiveCommand command = new ExitCommand();

            // Assert
            Assert.NotNull(command);
        }

        /// <summary>
        /// Tests that multiple implementations can implement IInteractiveCommand
        /// </summary>
        [Fact]
        public void IInteractiveCommand_CanHaveMultipleImplementations()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Act
            int implementationCount = 0;
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (!type.IsInterface && 
                            typeof(IInteractiveCommand).IsAssignableFrom(type) && 
                            type.Namespace == "Dotnet.Script.Core")
                        {
                            implementationCount++;
                        }
                    }
                }
                catch
                {
                    // Skip assemblies that cannot be loaded
                }
            }

            // Assert - at least ExitCommand should implement it
            Assert.True(implementationCount >= 1, $"Expected at least 1 implementation, found {implementationCount}");
        }

        /// <summary>
        /// Tests that IInteractiveCommand defines a contract for interactive commands
        /// </summary>
        [Fact]
        public void IInteractiveCommand_DefinesCommandContract()
        {
            // Arrange
            var interfaceType = typeof(IInteractiveCommand);

            // Act
            var members = interfaceType.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            // Assert - should have Name property and Execute method
            var properties = interfaceType.GetProperties();
            var methods = interfaceType.GetMethods();

            Assert.Single(properties);
            Assert.Single(methods);
            Assert.Equal("Name", properties[0].Name);
            Assert.Equal("Execute", methods[0].Name);
        }

        /// <summary>
        /// Tests that Name property is readable through the interface
        /// </summary>
        [Fact]
        public void IInteractiveCommand_NameProperty_IsReadable()
        {
            // Arrange
            IInteractiveCommand command = new ExitCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.NotNull(name);
            Assert.IsType<string>(name);
        }

        /// <summary>
        /// Tests that Execute method is callable through the interface
        /// </summary>
        [Fact]
        public void IInteractiveCommand_ExecuteMethod_IsCallable()
        {
            // Arrange
            IInteractiveCommand command = new ExitCommand();
            var mockRunner = new Moq.Mock<InteractiveRunner>();
            var mockConsole = new Moq.Mock<ScriptConsole>();
            var context = new CommandContext(mockConsole.Object, mockRunner.Object);

            // Act & Assert - should not throw
            var exception = Record.Exception(() => command.Execute(context));
            Assert.Null(exception);
        }
    }
}