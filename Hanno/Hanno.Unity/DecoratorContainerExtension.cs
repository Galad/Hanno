using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace Hanno.Unity
{
    public class DecoratorContainerExtension : UnityContainerExtension
    {
        private Dictionary<Type, List<Type>> _typeStacks;
        protected override void Initialize()
        {
            _typeStacks = new Dictionary<Type, List<Type>>();
            Context.Registering += AddRegistration;

            Context.Strategies.Add(
                new DecoratorBuildStrategy(_typeStacks),
                UnityBuildStage.PreCreation
            );
        }

        private void AddRegistration(object sender, RegisterEventArgs e)
        {
            if (e.TypeFrom == null || !e.TypeFrom.GetTypeInfo().IsInterface)
            {
                return;
            }

            List<Type> stack = null;
            if (!_typeStacks.ContainsKey(e.TypeFrom))
            {
                stack = new List<Type>();
                _typeStacks.Add(e.TypeFrom, stack);
            }
            else
            {
                stack = _typeStacks[e.TypeFrom];
            }

            stack.Add(e.TypeTo);
        }
    }
}
