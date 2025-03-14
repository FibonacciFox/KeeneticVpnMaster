using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace KeeneticVpnMaster.ViewModels.Pages
{
    public class StaticRouteItemViewModel : ReactiveObject
    {
        public string Name { get; }
        public List<StaticRouteItemViewModel> Children { get; }
        public StaticRouteItemViewModel? Parent { get; set; }

        private bool _isChecked = false;
        private bool _isUpdatingFromChild = false;
        
        /// <summary>
        /// Если пользователь вручную меняет значение, то обновляются все дочерние элементы.
        /// Если изменение инициировано изменением дочерних, то просто обновляется значение и уведомляется UI.
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    if (!_isUpdatingFromChild)
                    {
                        // Пользовательская инициатива: обновляем и детей, и сообщаем родителю
                        _isChecked = value;
                        this.RaisePropertyChanged(nameof(IsChecked));
                        UpdateChildren(value);
                        Parent?.Reevaluate();
                    }
                    else
                    {
                        // Программное обновление, вызванное изменением детей — не трогаем детей снова
                        _isChecked = value;
                        this.RaisePropertyChanged(nameof(IsChecked));
                    }
                }
            }
        }

        public StaticRouteItemViewModel(string name, IEnumerable<StaticRouteItemViewModel>? children = null, StaticRouteItemViewModel? parent = null)
        {
            Name = name;
            Parent = parent;
            Children = children?.ToList() ?? new List<StaticRouteItemViewModel>();

            // Устанавливаем текущий объект как родителя для всех дочерних элементов
            foreach (var child in Children)
            {
                child.Parent = this;
            }
        }

        /// <summary>
        /// Обновляет состояние всех дочерних элементов.
        /// Вызывается, когда пользователь изменяет состояние текущего элемента.
        /// </summary>
        private void UpdateChildren(bool value)
        {
            foreach (var child in Children)
            {
                child.IsChecked = value;  // Через свойство — вызывается логика обновления дочерних
            }
        }

        /// <summary>
        /// Переоценивает состояние родителя на основе состояний всех дочерних элементов.
        /// Если хотя бы один дочерний элемент выбран, родитель становится true; иначе — false.
        /// При этом изменение не вызывает повторного обновления детей.
        /// </summary>
        public void Reevaluate()
        {
            if (Children.Any())
            {
                bool newVal = Children.Any(c => c.IsChecked);
                _isUpdatingFromChild = true;
                if (_isChecked != newVal)
                {
                    _isChecked = newVal;
                    this.RaisePropertyChanged(nameof(IsChecked));
                    Parent?.Reevaluate(); // Рекурсивно переоцениваем состояние выше
                }
                _isUpdatingFromChild = false;
            }
        }
    }
}
