# Luokat ja tiedostot

[Takaisin yleiskuvaan](README.md) | [Toimintaketjut](Toimintaketjut.md)

Tässä osassa kuvataan käsin kirjoitetun sovelluskoodin luokat. Tiedostopolut ovat suhteessa `ShoppingMauiMvvm`-projektikansioon.

## Käynnistys ja sivurakenne

### `MauiProgram` – `MauiProgram.cs`

Staattisen `MauiProgram`-luokan `CreateMauiApp`-metodi rakentaa MAUI-sovelluksen. Se:

1. määrittää sovelluksen juuriluokaksi `App` ja rekisteröi fontit;
2. luo `HttpClient`-olion ja asettaa sille taustapalvelun perusosoitteen;
3. rekisteröi palvelut, sivut, näkymämallit ja `AppShell`-olion riippuvuuksien injektointia varten;
4. ottaa debug-lokittamisen käyttöön debug-käännöksessä ja palauttaa valmiin sovelluksen.

`HttpClient`, palvelut ja `AppShell` rekisteröidään **singleton**-elinkaarella: samaa oliota käytetään sovelluksen palvelusäiliössä. Sivut ja näkymämallit rekisteröidään **transient**-elinkaarella: palvelusäiliö luo uuden olion, kun kyseinen tyyppi pyydetään. Tämä ei tarkoita, että jo luotu sivu luotaisiin uudelleen aina, kun se tulee näkyviin.

### `App` – `App.xaml` ja `App.xaml.cs`

`App` edustaa MAUI-sovellusta. Se saa `AppShell`-olion rakentajassaan ja luo `CreateWindow`-metodissa ikkunan, jonka sisällöksi tulee kyseinen Shell. `App.xaml` yhdistää sovelluksen yhteiset väri- ja tyylitiedostot.

### `AppShell` – `AppShell.xaml` ja `AppShell.xaml.cs`

`AppShell` määrittää sovelluksen Shell-rakenteen eli sivujen ja reittien kokonaisuuden. XAML määrittää `MainPage`-sivun reitillä `MainPage`. C#-rakentaja rekisteröi `AddPage`-sivun reitin. Sivuille siirrytään näiden reittien avulla.

### `MainPage` – `MainPage.xaml` ja `MainPage.xaml.cs`

Pääsivu näyttää ostoslistan, valitun tuotteen keräämispainikkeen, lisäyssivulle siirtymisen sekä lataus- ja virhetilat. `CollectionView` näyttää tuotteet ja `RefreshView` tarjoaa listan päivitystoiminnon.

Rakentaja alustaa XAML-käyttöliittymän `InitializeComponent`-kutsulla ja asettaa saamansa `ShoplistViewModel`-olion sivun `BindingContext`-arvoksi. `BindingContext` kertoo sidonnoille, mistä niiden ominaisuudet ja komennot löytyvät. Kun sivu tulee näkyviin, `OnAppearing` käynnistää latauskomennon, jos komento on suoritettavissa.

### `AddPage` – `AddPage.xaml` ja `AddPage.xaml.cs`

Lisäyssivulla on kentät tuotteen nimelle ja määrälle, lisäyspainike, virheilmoitus sekä toimintoa ilmaiseva `ActivityIndicator`. Rakentaja alustaa XAML:n ja asettaa saamansa `AddItemViewModel`-olion sivun `BindingContext`-arvoksi. Syötteiden tarkistus ja tallennus tehdään näkymämallissa, ei sivuluokassa.

## Tietomalli

### `Shoplist` – `Models\Shoplist.cs`

`Shoplist` kuvaa yhtä ostoslistan tuotetta:

| Ominaisuus | Tyyppi | Merkitys |
| --- | --- | --- |
| `Id` | `int` | Tuotteen tunniste, jota käytetään esimerkiksi poistopyynnössä. |
| `Item` | `string` | Tuotteen nimi. |
| `Amount` | `int` | Tuotteen määrä kappaleina. |

Luokka on yksinkertainen tieto-olio: se ei itse hae tai tallenna tietoja. `Item`-ominaisuuden `null!`-alustus vaientaa kääntäjän null-varoituksen; se ei yksin tarkista nimeä. Lisäysnäkymämalli tarkistaa käyttäjän syötteen ennen lähettämistä.

## Näkymämallit

Molemmat näkymämallit perivät `BindableObject`-luokan. Kun ne kutsuvat `OnPropertyChanged`-metodia ominaisuuden muututtua, sidottu käyttöliittymä saa tiedon muutoksesta. Komentojen `ChangeCanExecute` puolestaan ilmoittaa, että niiden suoritettavuus on voinut muuttua.

### `ShoplistViewModel` – `ViewModels\ShoplistViewModel.cs`

Pääsivun näkymämalli saa rakentajassaan ostoslista- ja navigointipalvelun rajapinnat. Sen tärkeimmät ominaisuudet ovat:

| Ominaisuus | Tehtävä |
| --- | --- |
| `Shoplist` | Näytettävät tuotteet `ObservableCollection<Shoplist>`-kokoelmana. Kokoelma ilmoittaa esimerkiksi tuotteen poistamisesta. |
| `SelectedItem` | Käyttäjän valitsema tuote tai `null`. |
| `ErrorMessage` | Käyttöliittymässä näytettävä virheilmoitus. |
| `IsLoading` | Ilmaisee, että listaa ladataan tai tuotetta kerätään. |
| `CanCollectSelectedItem` | On tosi, kun tuote on valittu eikä lataus ole käynnissä. |

Näkymämallin komennot ovat:

- `LoadCommand`: kutsuu `LoadAsync`-metodia. Se hakee listan, tyhjentää valinnan onnistuneen haun jälkeen ja korvaa näytettävän kokoelman.
- `OpenAddItemCommand`: kutsuu `OpenAddItemAsync`-metodia, joka pyytää navigointipalvelua avaamaan lisäyssivun. Yksityinen `isNavigating`-tila estää saman navigointikomennon käynnistämisen uudelleen sen ollessa kesken.
- `CollectSelectedItemCommand`: kutsuu `CollectSelectedItemAsync`-metodia. Se poistaa valitun tuotteen palvelimelta, poistaa onnistumisen jälkeen tuotteen näkyvästä kokoelmasta ja yrittää hakea listan uudelleen.

Latauksen aikana komennot ovat rajoitettuja. Poistokomento edellyttää lisäksi valittua tuotetta. Virheistä kerrotaan `ErrorMessage`-ominaisuuden kautta, ja lataustila palautetaan `finally`-lohkossa.

### `AddItemViewModel` – `ViewModels\AddItemViewModel.cs`

Lisäyssivun näkymämalli saa samat palvelurajapinnat kuin pääsivun näkymämalli. `NewItemName` ja `NewItemQuantity` sisältävät kenttien tekstin. `ErrorMessage` sisältää mahdollisen virheilmoituksen ja `IsBusy` kertoo lisäyksen olevan käynnissä.

`AddItemCommand` suorittaa `AddItemAsync`-metodin, kun näkymämalli ei ole varattu. Metodi tarkistaa, että määrä on nollaa suurempi kokonaisluku ja nimi sisältää muutakin kuin tyhjää tilaa. Se poistaa nimen alusta ja lopusta välilyönnit `Trim`-metodilla, luo `Shoplist`-olion ja antaa sen tallennettavaksi palvelulle. Tallennuksen onnistuttua kentät tyhjennetään ja navigointipalvelua pyydetään palaamaan pääsivulle. Myös navigoinnin mahdollinen epäonnistuminen näytetään erikseen.

## Palvelut ja rajapinnat

### `IShoplistService` – `Services\IShoplistService.cs`

Rajapinta määrittelee näkymämalleille kolme asynkronista toimintoa:

- `GetAllAsync`: palauttaa tuotteet muodossa `IReadOnlyList<Shoplist>`.
- `AddAsync`: lisää sille annetun `Shoplist`-tuotteen.
- `CollectAsync`: kerää eli poistaa tuotteen annetun kokonaislukutunnisteen perusteella.

Metodeilla on valinnainen `CancellationToken`, joka mahdollistaa kutsun peruuttamisen sitä käyttävälle kutsujalle. Nykyiset näkymämallit käyttävät oletusarvoa eivätkä itse peruuta pyyntöjä.

### `ShoplistApiService` – `Services\ShoplistApiService.cs`

`ShoplistApiService` toteuttaa `IShoplistService`-rajapinnan ja saa `HttpClient`-olion rakentajassaan. Se lähettää HTTP-pyynnöt suhteellisilla osoitteilla; `HttpClient` yhdistää ne `MauiProgram`-luokassa asetettuun perusosoitteeseen.

| Metodi | HTTP-pyyntö | Tarkoitus |
| --- | --- | --- |
| `GetAllAsync` | `GET api/shoplist` | Hakee tuotteet ja muuntaa JSON-vastauksen listaksi. |
| `AddAsync` | `POST api/shoplist` | Lähettää tuotteen JSON-muodossa. |
| `CollectAsync` | `DELETE api/shoplist/{itemId}` | Pyytää poistamaan tunnisteella yksilöidyn tuotteen. |

`EnsureSuccessStatusCode` aiheuttaa poikkeuksen, jos HTTP-vastaus ei ilmoita onnistumista. Jos haun JSON-tulos on `null`, palvelu heittää erillisen poikkeuksen. Näkymämallit käsittelevät virheet ja näyttävät niitä käyttäjälle. Palvelun koodi määrittelee, mitä asiakas lähettää ja lukee; taustapalvelimen sisäistä tallennusta ei voi päätellä tästä projektista.

### `IAppNavigationService` – `Services\IAppNavigationService.cs`

Rajapinta määrittelee näkymämallien tarvitsemat siirtymät: `NavigateToAddItemAsync` avaa lisäyssivun ja `GoBackAsync` siirtyy takaisin listanäkymään. Näkymämallien ei tarvitse tuntea Shell-reittien yksityiskohtia.

### `ShellNavigationService` – `Services\ShellNavigationService.cs`

Palvelu toteuttaa navigointirajapinnan MAUI Shellin avulla. `NavigateToAddItemAsync` käyttää `AddPage`-reittiä ja `GoBackAsync` absoluuttista `//MainPage`-reittiä. Nimestään huolimatta `GoBackAsync` ei siis kutsu yleistä edelliselle sivulle palaamisen toimintoa, vaan navigoi nimenomaan pääsivulle.

## Alustakohtaiset luokat

`Platforms`-kansion tiedostot yhdistävät yhteisen MAUI-sovelluksen eri käyttöjärjestelmien käynnistysmekanismeihin. Ne eivät sisällä ostoslistan toimintalogiikkaa.

| Alusta | Luokka | Tehtävä |
| --- | --- | --- |
| Android | `MainActivity` | Androidin käynnistettävä MAUI-aktiviteetti. |
| Android | `MainApplication` | Luo yhteisen MAUI-sovelluksen kutsumalla `MauiProgram.CreateMauiApp`-metodia. |
| iOS ja Mac Catalyst | `Program` | Käynnistää käyttöjärjestelmän sovelluksen ja antaa sille `AppDelegate`-luokan. |
| iOS ja Mac Catalyst | `AppDelegate` | Luo yhteisen MAUI-sovelluksen. |
| iOS ja Mac Catalyst | `SceneDelegate` | Käyttää MAUI:n valmista ikkunanäkymän käsittelyä. |
| Windows | `ShoppingMaui.WinUI.App` | Alustaa Windowsin sovellusolion ja luo yhteisen MAUI-sovelluksen. Tämä on eri luokka kuin yhteinen `ShoppingMaui.App`. |

Alustojen manifestit ja muut asetustiedostot sisältävät käyttöjärjestelmäkohtaisia määrityksiä. Esimerkiksi Androidin manifestissa on verkkoyhteyden käyttöön tarvittava internet-lupa.