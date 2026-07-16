using System.Collections.ObjectModel;
using TestSuite.Views;

namespace TestSuite.Core;

public class CorePage : ContentPage
{
    static readonly string[] _controls =
    {
        "ActivityIndicator",
        "Button",
        "Border",
        "BoxView",
        "CarouselView",
        "CollectionView",
        "CheckBox",
        "DatePicker",
        "Editor",
        "Entry",
        "Image",
        "Label",
        "Picker",
        "ProgressBar",
        "RadioButton",
        "SearchBar",
        "Slider",
        "Stepper",
        "Switch",
        "TimePicker"
    };

    static readonly string[] _layouts =
    {
        "AbsoluteLayout",
        "Grid",
        "StackLayout",
        "VerticalStackLayout",
        "HorizontalStackLayout",
        "BindableLayout",
    };

    static readonly string[] _pages =
    {
        "ContentPage",
        "NavigationPage",
        "TabbedPage",
        "FlyoutPage",
    };

    static readonly string[] _shells =
    {
        "Shell",
        "FlyoutItem",
        "TabBar",
        "MenuItem",
        "ShellContent",
    };

    static readonly Style sectionHeaderStyle = new Style(typeof(Label))
    {
        Setters =
            {
                new Setter { Property = Label.FontSizeProperty, Value = 13d },
                new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
                new Setter { Property = Label.TextColorProperty, Value = Colors.White },
                new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromArgb("#444") },
                new Setter { Property = Label.PaddingProperty, Value = new Thickness(8,4) }
            }
    };

    // Single source of truth for the group name -> items pairs, used both to
    // seed _itemsSource and to drive the search filtering below.
    static readonly (string GroupName, string[] Items)[] _groups =
    {
        ("Controls", _controls),
        ("Layouts", _layouts),
        ("Pages", _pages),
        ("Shells", _shells)
    };

    ObservableCollection<ControlGroup> _itemsSource =
        new ObservableCollection<ControlGroup>(
            _groups.Select(g => new ControlGroup(g.GroupName, g.Items)));

    Entry? _searchEntry;

    public CorePage()
    {
        Title = "Home";
        Content = CreateRootLayout();
    }

    Grid CreateRootLayout()
    {
        Grid rootGrid = new Grid
        {
            RowSpacing = 5,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }
            }
        };

        _searchEntry = new Entry
        {
            Placeholder = "Enter a control name to search",
            Margin = new Thickness(8, 4),
            BackgroundColor = Colors.White,
            TextColor = Colors.Black
        };

        _searchEntry.TextChanged += OnEntryTextChanged;

        rootGrid.Add(_searchEntry, 0, 0);

        Button searchButton = new Button
        {
            Text = "Search",
            Margin = new Thickness(8, 4),
        };

        searchButton.Clicked += (s, e) =>
        {
            PerformNavigation(_searchEntry?.Text);
        };

        rootGrid.Add(searchButton, 0, 1);

        rootGrid.Add(CreateCollectionView(), 0, 2);

        return rootGrid;
    }

    private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Perform the search logic here
        // For example, you can filter the controls, layouts, pages, and shells based on the search text
        // and display the results in a ListView or CollectionView.
        string searchText = e.NewTextValue?.ToLower() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(searchText))
        {
            searchText = string.Empty;
        }

        searchText = searchText.ToLowerInvariant();

        _itemsSource.Clear();

        foreach (var group in _groups)
        {
            var filteredItems = group.Items
                .Where(item => item.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (filteredItems.Count != 0)
                _itemsSource.Add(new ControlGroup(group.GroupName, filteredItems));
        }
    }

    CollectionView CreateCollectionView()
    {
        CollectionView collectionView = new CollectionView
        {
            ItemsSource = _itemsSource,
            IsGrouped = true,
            SelectionMode = SelectionMode.Single,
            GroupHeaderTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    Margin = new Thickness(0, 8, 0, 0),
                    Style = sectionHeaderStyle
                };
                label.SetBinding(Label.TextProperty, "GroupName");

                return label;
            }),

            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    FontSize = 13,
                    TextColor = Color.FromArgb("#333"),
                    VerticalOptions = LayoutOptions.Center
                };
                label.SetBinding(Label.TextProperty, ".");

                return new Grid
                {
                    Padding = new Thickness(8, 4),
                    ColumnSpacing = 8,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star }
                    },
                    Children =
                    {
                        label
                    }
                };
            })
        };

        collectionView.SelectionChanged += (s, e) =>
        {
            if (e.CurrentSelection.Count > 0)
            {
                var selectedItem = e.CurrentSelection[0] as string;
                if (!string.IsNullOrEmpty(selectedItem))
                {
                    // Perform navigation to the selected control's page
                    PerformNavigation(selectedItem);
                }

                // Clear the selection
                (s as CollectionView)?.SelectedItem = null;
            }
        };

        return collectionView;
    }

    // Explicit registry of implemented control pages. Keeps navigation type-safe
    // and avoids reflecting on a "TestSuite.Views.{name}.{name}NavPage" naming
    // convention that throws a NullReferenceException for any page not yet built.
    static readonly Dictionary<string, Func<Page>> _navPageFactories = new()
    {
        ["Switch"] = () => new SwitchNavPage(),
    };

    void PerformNavigation(string? controlName)
    {
        if (controlName is null || !_navPageFactories.TryGetValue(controlName, out var createPage))
            return; // No page implemented for this control yet.

        Application.Current!.Windows[0].Page = createPage();
    }
}

public class ControlGroup : List<string>
{
    public string GroupName { get; set; }

    public ControlGroup(string groupName, IEnumerable<string> controls) : base(controls)
    {
        GroupName = groupName;
    }
}
