Wassup bitches
Her er en guide til hvordan man laver en ny view + hvordan man integrerer den ind i appen OwO


LAV NY VIEW GUIDE :)))
    STEP 1
Højreklik Views mappen og vælg Avalonia user control
Kald den for etellerandetView
Tada nu har du en .axaml fil og inden under den en .axaml.cs

    STEP 2
Højreklik ViewModels mappen og lav en .cs fil
Kald den for etellerandetViewModel
Tillykke! nu har du en .cs fil

    STEP 3
Forbind dem inde i ViewLocator.cs
Gør ligesom de andre der står derinde 
WOW! nu er din model forbundet med den rigtige .axaml fil

NAVIGATION MELLEM VIEWS GUIDE :((((

  PRE GUIDE INFO
Lige nu er det sat op så MainView holder styr på navigationen mellem views i vores app, det gør den med ContentControl der sørger for at den korrekte view bliver vist
Det er sådan så viewsne ikke blander hvilken CurrentPage man rent faktisk kigger på 
Det betyder så også at når man skifter mellem views, at man i Modellen skal bruge _mainViewModel.NavigateTo så det er mainViewModellens NavigateTo function der bliver brugt

    STEP 1
Sæt en public partial class op inde i din ViewModel ligesom det her exempel fra DashboardViewModel:

public partial class DashboardViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public DashboardViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    } 

    [RelayCommand]
    public void GoToChatBot()
    {
        _mainViewModel.NavigateTo(new ChatBotViewModel(_mainViewModel));
    }
    
    
}

self ændr den nederste relayCommand til at navigate til den viewmodel du nu har lavet

    STEP 2
inde i den tilsvarende .axaml fil forbind fx en knap med RelayCommanden ligesom i exemplet fra DashboardView.axaml

Kig kun på Command det den der er vigtig her

<Button Grid.Row="3"
                Background="RoyalBlue"
                Padding="10"
                CornerRadius="10"
                HorizontalAlignment="Right"
                VerticalAlignment="Stretch"
                Margin="10,0,10,10"
                Command="{Binding GoToChatBotCommand}">
            <PathIcon Data="{StaticResource ChatRegular}" Height="80" Width="80"/>
        </Button>


    STEP 3
Profit (hvis jeg har husket at vise alle steps, hvem ved)
