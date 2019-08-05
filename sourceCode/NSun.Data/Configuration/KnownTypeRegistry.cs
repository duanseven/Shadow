using System;
using System.Collections.Generic;

namespace NSun.Data.Configuration
{
    /// <summary>
    /// Registry for all known types for serializing QueryCriteria class.
    /// </summary>
    public sealed class KnownTypeRegistry
    {
        /// <summary>
        /// The singleton.
        /// </summary>
        public static readonly KnownTypeRegistry Instance;

        private readonly List<Type> _knownTypes;
        private readonly object _knownTypesLock;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownTypeRegistry"/> class.
        /// </summary>
        private KnownTypeRegistry()
        {
            _knownTypes = new List<Type>();
            _knownTypesLock = new object();
        }

        /// <summary>
        /// Initializes the <see cref="KnownTypeRegistry"/> class.
        /// </summary>
        static KnownTypeRegistry()
        {
            Instance = new KnownTypeRegistry();
            AddPredefinedKnownTypes();
        }

        #endregion

        #region Non-Public Properties

        /// <summary>
        /// Gets or sets the known types.
        /// </summary>
        /// <value>The known types.</value>
        public Type[] KnownTypes { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a known type to registry.
        /// </summary>
        /// <param name="type">The known type to add.</param>
        /// <returns></returns>
        public bool AddKnownType(Type type)
        {
            if (type != null)
            {
                if (!_knownTypes.Contains(type))
                {
                    lock (_knownTypesLock)
                    {
                        if (!_knownTypes.Contains(type))
                        {
                            _knownTypes.Add(type);
                            KnownTypes = _knownTypes.ToArray();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Remove a known type from registry.
        /// </summary>
        /// <param name="type">The known type to remove.</param>
        /// <returns></returns>
        public bool RemoveKnownType(Type type)
        {
            if (type != null)
            {
                if (_knownTypes.Contains(type))
                {
                    lock (_knownTypesLock)
                    {
                        if (_knownTypes.Contains(type))
                        {
                            var result = _knownTypes.Remove(type);
                            KnownTypes = _knownTypes.ToArray();
                            return result;
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region Non-Public Methods

        private static void AddPredefinedKnownTypes()
        {
            var knownTypes = new[]
                       {
                           typeof(Condition),
                           typeof(Assignment),
                           typeof(ParameterEqualsCondition),
                           typeof(NullExpression),
                           typeof(Expression), 
                           typeof(ExpressionClip), 
                           typeof(ExpressionCollection),
                           typeof(ParameterExpression), 
                           typeof(QueryColumn), 
                           typeof(VersionQueryColumn),
                           typeof(IdQueryColumn), 
                           typeof(RelationQueryColumn), 
                           typeof(ExpressionClip),
                           typeof(BaseEntity),
                           typeof(BaseEntityRefObject),
                           typeof(SprocEntity),
                           typeof(SelectSqlSection),
                           typeof(InsertSqlSection),
                           typeof(UpdateSqlSection),
                           typeof(DeleteSqlSection),
                           typeof(CustomSqlSection),
                           //typeof(UpdateSqlSection<>),
                           //typeof(SelectSqlSection<>),
                           //typeof(DeleteSqlSection<>),
                           typeof(SubQuery),
                           typeof(FromClip),
                           typeof(OrderByClip), 
                           typeof(QueryCriteria)
                       };

            foreach (var type in knownTypes)
            {
                Instance.AddKnownType(type);
            }
        }

        #endregion
    }
}
