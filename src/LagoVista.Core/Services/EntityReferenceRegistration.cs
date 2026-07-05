using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace LagoVista
{
    public interface IEntityReferenceRegistration
    {
        string Key { get; }

        Type SourceType { get; }

        Type ReferencingType { get; }

        LambdaExpression HeaderSelectorExpression { get; }

        LambdaExpression CreateReferencePredicate(string sourceId);

        IEnumerable<EntityHeader> GetHeaders(object referencingEntity);
    }

    internal sealed class EntityReferenceRegistration<TSource, TRef> : IEntityReferenceRegistration
        where TSource : class, IIDEntity
        where TRef : class, IIDEntity
    {
        private readonly Func<string, Expression<Func<TRef, bool>>> _referencePredicateFactory;
        private readonly Func<TRef, IEnumerable<EntityHeader>> _headerSelector;

        public EntityReferenceRegistration(string key, Func<string, Expression<Func<TRef, bool>>> referencePredicateFactory, Expression<Func<TRef, IEnumerable<EntityHeader>>> headerSelectorExpression)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("A relationship registration key is required.", nameof(key));

            Key = key.Trim();
            _referencePredicateFactory = referencePredicateFactory ?? throw new ArgumentNullException(nameof(referencePredicateFactory));
            HeaderSelectorExpression = headerSelectorExpression ?? throw new ArgumentNullException(nameof(headerSelectorExpression));
            _headerSelector = headerSelectorExpression.Compile();
        }

        public string Key { get; }

        public Type SourceType => typeof(TSource);

        public Type ReferencingType => typeof(TRef);

        public LambdaExpression HeaderSelectorExpression { get; }

        public LambdaExpression CreateReferencePredicate(string sourceId)
        {
            if (String.IsNullOrWhiteSpace(sourceId)) throw new ArgumentException("The source entity id is required.", nameof(sourceId));

            var predicate = _referencePredicateFactory(sourceId.Trim());

            if (predicate == null)
                throw new InvalidOperationException($"Relationship registration '{Key}' returned a null reference predicate.");

            return predicate;
        }

        public IEnumerable<EntityHeader> GetHeaders(object referencingEntity)
        {
            if (referencingEntity == null) throw new ArgumentNullException(nameof(referencingEntity));

            if (!(referencingEntity is TRef typedEntity))
            {
                throw new ArgumentException(
                    $"Relationship registration '{Key}' expected an entity of type '{typeof(TRef).FullName}', but received '{referencingEntity.GetType().FullName}'.",
                    nameof(referencingEntity));
            }

            return _headerSelector(typedEntity)?.Where(header => header != null) ?? Enumerable.Empty<EntityHeader>();
        }
    }

    public static class EntityReferenceRegistry
    {
        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<Type, List<IEntityReferenceRegistration>> _registrationsBySourceType = new Dictionary<Type, List<IEntityReferenceRegistration>>();
        private static readonly HashSet<string> _registrationKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static bool _isFrozen;

        public static bool IsFrozen
        {
            get
            {
                lock (_syncRoot)
                    return _isFrozen;
            }
        }

        public static void Register<TSource, TRef>(string key, Func<string, Expression<Func<TRef, bool>>> referencePredicateFactory, Expression<Func<TRef, IEnumerable<EntityHeader>>> headerSelector)
            where TSource : class, IIDEntity
            where TRef : class, IIDEntity
        {
            var registration = new EntityReferenceRegistration<TSource, TRef>(key, referencePredicateFactory, headerSelector);

            lock (_syncRoot)
            {
                if (_isFrozen)
                    throw new InvalidOperationException($"Entity reference registration '{registration.Key}' cannot be added because the registry has been frozen.");

                if (!_registrationKeys.Add(registration.Key))
                    throw new InvalidOperationException($"An entity reference registration with the key '{registration.Key}' has already been added.");

                if (!_registrationsBySourceType.TryGetValue(registration.SourceType, out var registrations))
                {
                    registrations = new List<IEntityReferenceRegistration>();
                    _registrationsBySourceType.Add(registration.SourceType, registrations);
                }

                registrations.Add(registration);
            }
        }

        public static IReadOnlyList<IEntityReferenceRegistration> GetRegistrations<TSource>() where TSource : class, IIDEntity
        {
            return GetRegistrations(typeof(TSource));
        }

        public static IReadOnlyList<IEntityReferenceRegistration> GetRegistrations(Type sourceType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));

            lock (_syncRoot)
            {
                if (!_registrationsBySourceType.TryGetValue(sourceType, out var registrations))
                    return Array.Empty<IEntityReferenceRegistration>();

                return new ReadOnlyCollection<IEntityReferenceRegistration>(registrations.ToList());
            }
        }

        public static IReadOnlyList<IEntityReferenceRegistration> GetAllRegistrations()
        {
            lock (_syncRoot)
            {
                var registrations = _registrationsBySourceType.Values.SelectMany(items => items).ToList();
                return new ReadOnlyCollection<IEntityReferenceRegistration>(registrations);
            }
        }

        public static void Freeze()
        {
            lock (_syncRoot)
                _isFrozen = true;
        }
    }
}