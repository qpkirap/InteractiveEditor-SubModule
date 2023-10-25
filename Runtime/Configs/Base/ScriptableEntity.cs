using System;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class ScriptableEntity : ScriptableObject, IEntity
    {
        [HideInInspector][SerializeField] protected string id;
        [HideInInspector][SerializeField] protected string title;

        public string Id => id;
        public string Title => title;

        public void SetId(string id)
        {
            this.id = id;
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public void GenerateId(bool force = false)
        {
            if (string.IsNullOrEmpty(id) || force)
            {
                id = GUID.Generate().ToString();

                Debug.Log($"Created Scriptable Entity: {id}");
            }
        }

        public virtual object Clone()
        {
            var item = Instantiate(this);

            item.GenerateId(force: true);
            item.AddCloneSuffix();

            return item;
        }

        public void AddCloneSuffix()
        {
            title += " (Clone)";
        }

        public void RemoveCloneSuffix()
        {
            title = title.Replace(" (Clone)", "");
            name = name.Replace("(Clone)", "");
        }

        #region Create

        /// <summary>
        /// Создание экземпляра <see cref="ScriptableEntity"/>
        /// </summary>
        public static ScriptableEntity Create(Type type)
        {
            return Create<ScriptableEntity>(type);
        }

        /// <summary>
        /// Создание экземпляра <see cref="ScriptableEntity"/>
        /// </summary>
        public static TEntity Create<TEntity>()
            where TEntity : class, IEntity
        {
            return Create<TEntity>(typeof(TEntity));
        }

        /// <summary>
        /// Создание экземпляра <see cref="ScriptableEntity"/>
        /// </summary>
        /// <param name="type">Тип создаваемого объекта</param>
        public static TEntity Create<TEntity>(Type type)
            where TEntity : class, IEntity
        {
            var entity = CreateInstance(type) as TEntity;

            entity.GenerateId();

            return entity;
        }

        #endregion Create

        #region IEquatable

        public bool Equals(IEntity other)
        {
            return other is not null
                && id == other.Id;
        }

        public override bool Equals(object other)
        {
            return other is ScriptableEntity entity
                && Equals(entity);
        }

        public override int GetHashCode()
        {
            return id?.GetHashCode() ?? 0;
        }

        #endregion IEquatable
    }
}