using System.Collections.ObjectModel;

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

    ObservableCollection<ControlGroup> _itemsSource =
        new ObservableCollection<ControlGroup>
    {
        new ControlGroup("Controls", _controls),
        new ControlGroup("Layouts", _layouts),
        new ControlGroup("Pages", _pages),
        new ControlGroup("Shells", _shells)
    };

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

        var filteredControls = _controls.Where(c => c.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
        var filteredLayouts = _layouts.Where(l => l.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
        var filteredPages = _pages.Where(p => p.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
        var filteredShells = _shells.Where(s => s.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();

        _itemsSource.Clear();

        if (filteredControls.Count != 0)
            _itemsSource.Add(new ControlGroup("Controls", filteredControls));

        if (filteredLayouts.Count != 0)
            _itemsSource.Add(new ControlGroup("Layouts", filteredLayouts));

        if (filteredPages.Count != 0)
            _itemsSource.Add(new ControlGroup("Pages", filteredPages));

        if (filteredShells.Count != 0)
            _itemsSource.Add(new ControlGroup("Shells", filteredShells));

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

    void PerformNavigation(string? controlName)
    {
        // Implement navigation logic here based on the selected control name

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
