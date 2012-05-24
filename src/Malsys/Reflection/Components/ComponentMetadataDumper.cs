/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Malsys.Evaluators;
using Malsys.Processing.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Reflection.Components {
	public class ComponentMetadataDumper {

		public ComponentMetadata GetMetadata(IComponent component, IMessageLogger logger, IXmlDocReader xmlDocReader = null) {

			Contract.Requires<ArgumentNullException>(component != null);
			Contract.Requires<ArgumentNullException>(logger != null);
			Contract.Ensures(Contract.Result<ComponentMetadata>() != null);

			return GetMetadata(component.GetType(), logger, xmlDocReader);

		}

		public ComponentMetadata GetMetadata(Type componentType, IMessageLogger logger, IXmlDocReader xmlDocReader = null) {

			Contract.Requires<ArgumentNullException>(componentType != null);
			Contract.Requires<ArgumentNullException>(logger != null);
			Contract.Ensures(Contract.Result<ComponentMetadata>() != null);

			if (!typeof(IComponent).IsAssignableFrom(componentType)) {
				logger.LogMessage(Message.InvalidComponentType, componentType.FullName, typeof(IComponent).FullName);
			}


			string name = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(componentType, "name") : null;
			string group = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(componentType, "group") : null;
			string summary = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(componentType) : null;

			bool isInstantiable, hasMsgCtor;
			return new ComponentMetadata(componentType,
				getGettableProperties(componentType, logger, xmlDocReader).ToImmutableList(),
				getSettableProperties(componentType, logger, xmlDocReader).ToImmutableList(),
				getSettableSymbolsProperties(componentType, logger, xmlDocReader).ToImmutableList(),
				getConnectableProperties(componentType, logger, xmlDocReader).ToImmutableList(),
				getCallableFunctions(componentType, logger, xmlDocReader).ToImmutableList(),
				getInterpretationMethods(componentType, logger, xmlDocReader).ToImmutableList(),
				getConstructor(componentType, logger, out isInstantiable, out hasMsgCtor),
				hasMsgCtor, isInstantiable, true,
				name, group, summary);

		}

		private IEnumerable<ComponentGettablePropertyMetadata> getGettableProperties(Type type, IMessageLogger logger, IXmlDocReader xmlDocReader) {

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

				string summary = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(propInfo) : null;

				yield return new ComponentGettablePropertyMetadata(propInfo.GetAccessNames().ToImmutableList(), propInfo, propInfoAndAttr.Item2.IsGettableBeforeInitialiation, summary);
			}

		}

		private IEnumerable<ComponentSettablePropertyMetadata> getSettableProperties(Type type, IMessageLogger logger, IXmlDocReader xmlDocReader) {

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

				string summary = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(propInfo) : null;
				string excpected = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(propInfo, "expected") : null;
				string def = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(propInfo, "default") : null;

				yield return new ComponentSettablePropertyMetadata(propInfo.GetAccessNames().ToImmutableList(), propInfo, propInfoAndAttr.Item2.IsMandatory, summary, excpected, def);
			}

		}

		private IEnumerable<ComponentSettableSybolsPropertyMetadata> getSettableSymbolsProperties(Type type, IMessageLogger logger, IXmlDocReader xmlDocReader) {

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

				string summary = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(propInfo) : null;

				yield return new ComponentSettableSybolsPropertyMetadata(propInfo.GetAccessNames().ToImmutableList(), propInfo, propInfoAndAttr.Item2.IsMandatory, summary);
			}

		}

		private IEnumerable<ComponentConnectablePropertyMetadata> getConnectableProperties(Type type, IMessageLogger logger, IXmlDocReader xmlDocReader) {

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

				string summary = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(propInfo) : null;

				yield return new ComponentConnectablePropertyMetadata(propInfo.GetAccessNames().ToImmutableList(), propInfo, propInfoAndAttr.Item2.IsOptional,
					propInfoAndAttr.Item2.AllowMultiple, summary);
			}

		}

		private IEnumerable<ComponentCallableFunctionMetadata> getCallableFunctions(Type type, IMessageLogger logger, IXmlDocReader xmlDocReader) {

			foreach (var propInfoAndAttr in type.GetMethodsHavingAttr<UserCallableFunctionAttribute>()) {

				MethodInfo methodInfo = propInfoAndAttr.Item1;
				var attr = propInfoAndAttr.Item2;

				var prms = methodInfo.GetParameters();
				if (prms.Length != 2 || prms[0].ParameterType != typeof(IValue[]) || prms[1].ParameterType != typeof(IExpressionEvaluatorContext)) {
					logger.LogMessage(Message.InvalidParamsOfCallableFun, methodInfo.Name, type.ToString(), typeof(IValue[]).FullName, typeof(IExpressionEvaluatorContext).FullName);
					continue;
				}

				if (!typeof(IValue).IsAssignableFrom(methodInfo.ReturnType)) {
					logger.LogMessage(Message.InvalidReturnTypeOfCallableFun, methodInfo.Name, type.ToString(), typeof(IValue).FullName);
					continue;
				}

				string summary = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(methodInfo) : null;
				string paramsDoc = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(methodInfo, "parameters") : null;

				yield return new ComponentCallableFunctionMetadata(methodInfo.GetAccessNames().ToImmutableList(), methodInfo, attr.ParamsCount,
					attr.ParamsTypesCyclicPattern.ToImmutableList(), attr.IsCallableBeforeInitialiation, summary, paramsDoc);
			}

		}

		private IEnumerable<ComponentInterpretationMethodMetadata> getInterpretationMethods(Type type, IMessageLogger logger, IXmlDocReader xmlDocReader) {

			foreach (var propInfoAndAttr in type.GetMethodsHavingAttr<SymbolInterpretationAttribute>()) {

				MethodInfo methodInfo = propInfoAndAttr.Item1;
				var attr = propInfoAndAttr.Item2;

				var prms = methodInfo.GetParameters();
				if (prms.Length != 1 || prms[0].ParameterType != typeof(ArgsStorage)) {
					logger.LogMessage(Message.InvalidParamsOfSymbolInterpret, methodInfo.Name, type.ToString(), typeof(ArgsStorage).FullName);
					continue;
				}

				if (methodInfo.ReturnType != typeof(void)) {
					logger.LogMessage(Message.InvalidReturnTypeOfSymbolInterpret, methodInfo.Name, type.ToString());
					continue;
				}

				string summary = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(methodInfo) : null;
				string paramsDoc = xmlDocReader != null ? xmlDocReader.GetXmlDocumentationAsString(methodInfo, "parameters") : null;

				yield return new ComponentInterpretationMethodMetadata(methodInfo.GetAccessNames().ToImmutableList(), methodInfo, attr.ParametersCount,
					attr.RequiredParametersCount, summary, paramsDoc);
			}

		}

		private ConstructorInfo getConstructor(Type type, IMessageLogger logger, out bool isInstantiable, out bool hasMessageCtor) {

			isInstantiable = !type.IsInterface && !type.IsAbstract;

			if (!isInstantiable) {
				hasMessageCtor = false;
				return null;
			}

			var ctorInfo = type.GetConstructor(new Type[] { typeof(IMessageLogger) });
			hasMessageCtor = ctorInfo != null;
			if (!hasMessageCtor) {
				ctorInfo = type.GetConstructor(Type.EmptyTypes);
				if (ctorInfo == null) {
					logger.LogMessage(Message.ComponentParamlessCtorMissing, type.FullName);
					return null;
				}
			}

			return ctorInfo;

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

			[Message(MessageType.Error, "Method `{0}` on component `{1}` marked as callable function has invalid parameters. " +
				"Callable function must have two parameters with types `{2}` and `{3}` respectively.")]
			InvalidParamsOfCallableFun,
			[Message(MessageType.Error, "Method `{0}` on component `{1}` marked as callable function has invalid return type. Return type must be assignable to `{2}`.")]
			InvalidReturnTypeOfCallableFun,

			[Message(MessageType.Error, "Method `{0}` on component `{1}` marked as symbol interpretation has invalid parameters. " +
				"Symbol interpretation must have one parameter of type `{2}`.")]
			InvalidParamsOfSymbolInterpret,
			[Message(MessageType.Error, "Method `{0}` on component `{1}` marked as symbol interpretation has invalid return type. Return type must be void.")]
			InvalidReturnTypeOfSymbolInterpret,

			[Message(MessageType.Error, "Component `{0}` does not have parameter-less constructor.")]
			ComponentParamlessCtorMissing,

		}


	}
}
