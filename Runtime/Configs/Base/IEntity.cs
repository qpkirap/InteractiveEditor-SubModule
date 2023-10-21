using System;

namespace Module.InteractiveEditor.Configs
{
    public interface IEntity : ICloneable, IEquatable<IEntity>
    {
        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Название сущности
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Задать идентификатор сущности
        /// </summary>
        /// <param name="id">Новый идентификатор сущности</param>
        void SetId(string id);

        /// <summary>
        /// Задать имя сущности
        /// </summary>
        /// <param name="title">Новое имя сущности</param>
        void SetTitle(string title);

        /// <summary>
        /// Сгенерировать идентификатор сущности
        /// </summary>
        /// <param name="force">Принудительно сгенерировать новый идентификатор</param>
        void GenerateId(bool force = false);

        void RemoveCloneSuffix();
    }
}