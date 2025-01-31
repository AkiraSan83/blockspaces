#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Injection;
using Ninject.Planning.Directives;
using Ninject.Selection;
#endregion

namespace Ninject.Planning.Strategies
{
    /// <summary>
    /// Adds directives to plans indicating which properties should be injected during activation.
    /// </summary>
    public class PropertyReflectionStrategy : NinjectComponent, IPlanningStrategy
    {
        /// <summary>
        /// Gets the selector component.
        /// </summary>
        public ISelector Selector { get; private set; }

        /// <summary>
        /// Gets the injector factory component.
        /// </summary>
        public IInjectorFactory InjectorFactory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyReflectionStrategy"/> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public PropertyReflectionStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Ensure.ArgumentNotNull(selector, "selector");
            Ensure.ArgumentNotNull(injectorFactory, "injectorFactory");

            Selector = selector;
            InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Adds a <see cref="PropertyInjectionDirective"/> to the plan for each property
        /// that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            Ensure.ArgumentNotNull(plan, "plan");

            foreach (PropertyInfo property in Selector.SelectPropertiesForInjection(plan.Type))
                plan.Add(new PropertyInjectionDirective(property, InjectorFactory.Create(property)));
        }
    }
}