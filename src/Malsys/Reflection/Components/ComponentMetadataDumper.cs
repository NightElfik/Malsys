﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.Processing.Components;
using Malsys.SemanticModel.Evaluated;
using System.Diagnostics.Contracts;
using Malsys.SemanticModel;

namespace Malsys.Reflection.Components {
	public class ComponentMetadataDumper {


		public ComponentMetadata GetMetadata(IComponent component, IMessageLogger logger) {

			Contract.Requires<ArgumentNullException>(component != null);
			Contract.Requires<ArgumentNullException>(logger != null);
			Contract.Ensures(Contract.Result<ComponentMetadata>() != null);

			return GetMetadata(component.GetType(), logger);

		}

		public ComponentMetadata GetMetadata(Type componentType, IMessageLogger logger) {

			Contract.Requires<ArgumentNullException>(componentType != null);
			Contract.Requires<ArgumentNullException>(logger != null);
			Contract.Ensures(Contract.Result<ComponentMetadata>() != null);

			if (!typeof(IComponent).IsAssignableFrom(componentType)) {
				logger.LogMessage(Message.InvalidComponentType, componentType.FullName, typeof(IComponent).FullName);
			}


			return new ComponentMetadata(componentType,
				getGettableProperties(componentType, logger).ToImmutableList(),
				getSettableProperties(componentType, logger).ToImmutableList(),
				getSettableSymbolsProperties(componentType, logger).ToImmutableList(),
				getConnectableProperties(componentType, logger).ToImmutableList(),
				getCallableFunctions(componentType, logger).ToImmutableList(),
				getConstructor(componentType, logger));

		}

		private IEnumerable<ComponentGettablePropertyMetadata> getGettableProperties(Type type, IMessageLogger logger) {

			foreach (var propInfoAndAttr in type.GetPropertiesHavingAttr<UserGettableAttribute>()) {
				PropertyInfo propInfo = propInfoAndAttr.Item1;

				if (!propInfo.CanRead) {
					logger.LogMessage(Message.GettablePropMissingGetter, propInfo.Name, type.ToString());
					continue;
				}

				if (!typeof(IValue).IsAssignableFrom(propInfo.PropertyType)) {
					logger.LogMessage(Message.GettablePropTypeError, propInfo.Name, type.ToString(), typeof(IValue).FullName);
					continue;
				}

				var names = getAliases(propInfo).ToList();
				names.Add(propInfo.Name);
				yield return new ComponentGettablePropertyMetadata(names.ToImmutableList(), propInfo);
			}
		}

		private IEnumerable<ComponentSettablePropertyMetadata> getSettableProperties(Type type, IMessageLogger logger) {

			foreach (var propInfoAndAttr in type.GetPropertiesHavingAttr<UserSettableAttribute>()) {
				PropertyInfo propInfo = propInfoAndAttr.Item1;

				if (!propInfo.CanWrite) {
					logger.LogMessage(Message.SettablePropMissingSetter, propInfo.Name, type.ToString());
					continue;
				}

				if (!typeof(IValue).IsAssignableFrom(propInfo.PropertyType)) {
					logger.LogMessage(Message.SettablePropTypeError, propInfo.Name, type.ToString(), typeof(IValue).FullName);
					continue;
				}

				var names = getAliases(propInfo).ToList();
				names.Add(propInfo.Name);
				yield return new ComponentSettablePropertyMetadata(names.ToImmutableList(), propInfo, propInfoAndAttr.Item2.IsMandatory);
			}
		}

		private IEnumerable<ComponentSettableSybolsPropertyMetadata> getSettableSymbolsProperties(Type type, IMessageLogger logger) {

			foreach (var propInfoAndAttr in type.GetPropertiesHavingAttr<UserSettableSybolsAttribute>()) {
				PropertyInfo propInfo = propInfoAndAttr.Item1;

				if (!propInfo.CanWrite) {
					logger.LogMessage(Message.SettablePropMissingSetter, propInfo.Name, type.ToString());
					continue;
				}

				if (propInfo.PropertyType != typeof(ImmutableList<Symbol<IValue>>)) {
					logger.LogMessage(Message.SettableSymbolsPropTypeError, propInfo.Name, type.ToString(), typeof(ImmutableList<Symbol<IValue>>).GetPrettyGenericName());
					continue;
				}

				var names = getAliases(propInfo).ToList();
				names.Add(propInfo.Name);
				yield return new ComponentSettableSybolsPropertyMetadata(names.ToImmutableList(), propInfo, propInfoAndAttr.Item2.IsMandatory);
			}
		}

		private IEnumerable<ComponentConnectablePropertyMetadata> getConnectableProperties(Type type, IMessageLogger logger) {

			foreach (var propInfoAndAttr in type.GetPropertiesHavingAttr<UserConnectableAttribute>()) {
				PropertyInfo propInfo = propInfoAndAttr.Item1;

				if (!propInfo.CanWrite) {
					logger.LogMessage(Message.ConnectablePropMissingSetter, propInfo.Name, type.ToString());
					continue;
				}

				if (!typeof(IComponent).IsAssignableFrom(propInfo.PropertyType)) {
					logger.LogMessage(Message.ConnectablePropTypeError, propInfo.Name, type.ToString(), typeof(IComponent).FullName);
					continue;
				}

				var names = getAliases(propInfo).ToList();
				names.Add(propInfo.Name);
				yield return new ComponentConnectablePropertyMetadata(names.ToImmutableList(), propInfo, propInfoAndAttr.Item2.IsOptional, propInfoAndAttr.Item2.AllowMultiple);
			}
		}

		private IEnumerable<ComponentCallableFunctionMetadata> getCallableFunctions(Type type, IMessageLogger logger) {

			foreach (var propInfoAndAttr in type.GetMethodsHavingAttr<UserCallableFunctionAttribute>()) {
				MethodInfo methodInfo = propInfoAndAttr.Item1;

				var prms = methodInfo.GetParameters();
				if (prms.Length != 1 || prms[0].ParameterType != typeof(ArgsStorage)) {
					logger.LogMessage(Message.InvalidParamsOfCallableFun, methodInfo.Name, type.ToString(), typeof(ArgsStorage).FullName);
					continue;
				}

				if (!typeof(IValue).IsAssignableFrom(methodInfo.ReturnType)) {
					logger.LogMessage(Message.InvalidReturnTypeOfCallableFun, methodInfo.Name, type.ToString(), typeof(IValue).FullName);
					continue;
				}

				var names = getAliases(methodInfo).ToList();
				names.Add(methodInfo.Name);
				yield return new ComponentCallableFunctionMetadata(names.ToImmutableList(), methodInfo);
			}
		}

		private ConstructorInfo getConstructor(Type type, IMessageLogger logger) {

			var ctorInfo = type.GetConstructor(Type.EmptyTypes);

			if (ctorInfo == null) {
				logger.LogMessage(Message.ComponentParamlessCtorMissing, type.FullName);
				return null;
			}

			return ctorInfo;

		}

		private IEnumerable<string> getAliases(MemberInfo memberInfo, bool inherit = true) {

			foreach (AliasAttribute aliasAttr in memberInfo.GetCustomAttributes(typeof(AliasAttribute), inherit)) {
				foreach (var alias in aliasAttr.Aliases) {
					yield return alias;
				}
			}

		}


		public enum Message {

			[Message(MessageType.Error, "Invalid component type `{0}`. Component does not implement required interface `{1}`.")]
			InvalidComponentType,

			[Message(MessageType.Error, "Property `{0}` on component `{1}` marked as gettable is not readable (getter missing).")]
			GettablePropMissingGetter,
			[Message(MessageType.Error, "Type of property `{0}` on component `{1}` marked as gettable is not assignable to `{2}`.")]
			GettablePropTypeError,

			[Message(MessageType.Error, "Property `{0}` on component `{1}` marked as settable is not writable (setter missing).")]
			SettablePropMissingSetter,
			[Message(MessageType.Error, "Type of property `{0}` on component `{1}` marked as settable is not assignable to `{2}`.")]
			SettablePropTypeError,
			[Message(MessageType.Error, "Type of property `{0}` on component `{1}` marked as settable symbols is not type of `{2}`.")]
			SettableSymbolsPropTypeError,

			[Message(MessageType.Error, "Property `{0}` on component `{1}` marked as connectable is not writable (setter missing).")]
			ConnectablePropMissingSetter,
			[Message(MessageType.Error, "Type of Property `{0}` on component `{1}` marked as connectable is not assignable to `{2}`.")]
			ConnectablePropTypeError,

			[Message(MessageType.Error, "Method `{0}` on component `{1}` marked as callable function has invalid parameters. Callable function can have only one parameter of type `{2}`.")]
			InvalidParamsOfCallableFun,
			[Message(MessageType.Error, "Method `{0}` on component `{1}` marked as callable function has invalid return type. Return type has to be assignable to `{2}`.")]
			InvalidReturnTypeOfCallableFun,

			[Message(MessageType.Error, "Component `{0}` does not have parameter-less constructor.")]
			ComponentParamlessCtorMissing,

		}


	}
}
