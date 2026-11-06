# ShoppingMauiMvvm – dokumentaatio

ShoppingMauiMvvm on .NET MAUI -sovellus, jolla käyttäjä voi tarkastella ostoslistaa, lisätä tuotteita ja poistaa listalta kerätyn tuotteen. Sovellus hakee ja tallentaa tiedot erillisessä palvelimessa olevan HTTP-rajapinnan kautta. Tämä projekti sisältää asiakassovelluksen, ei palvelimen toteutusta tai tietokantaa.

Dokumentaatio on tarkoitettu ohjelmoinnin perusteet, oliot ja C#-kielen perusrakenteet tuntevalle lukijalle.

## Sisältö

1. Tämä sivu esittelee sovelluksen rakenteen ja keskeiset käsitteet.
2. [Luokat ja tiedostot](Luokat.md) kuvaa kunkin sovellusluokan tehtävän.
3. [Toimintaketjut](Toimintaketjut.md) näyttää vaihe vaiheelta, miten tietojen lataaminen, lisääminen ja poistaminen etenevät.

## Yleiskuva arkkitehtuurista

Sovellus noudattaa **MVVM-mallia** (Model–View–ViewModel). Ajatuksena on erottaa tiedot, käyttöliittymä ja käyttöliittymän toimintalogiikka toisistaan.

```text
Käyttäjä
   |
   v
View: MainPage / AddPage            XAML ja sivujen C#-luokat
   |
   | tietojen sidonta ja komennot
   v
ViewModel: ShoplistViewModel / AddItemViewModel
   |                              |
   | IShoplistService             | IAppNavigationService
   v                              v
ShoplistApiService                ShellNavigationService
   |                              |
   v                              v
HTTP-rajapinta                    MAUI Shell ja sivujen reitit

Model: Shoplist kuvaa yhden tuotteen tietoja.
```

- **Model** eli tietomalli kuvaa sovelluksen käsittelemiä tietoja. `Shoplist` sisältää yhden tuotteen tunnisteen, nimen ja määrän.
- **View** eli näkymä näyttää tiedot käyttäjälle. `MainPage` ja `AddPage` muodostuvat XAML-käyttöliittymästä ja siihen liittyvästä C#-koodista.
- **ViewModel** eli näkymämalli sisältää näkymän tilan ja toiminnot, kuten valitun tuotteen, virheilmoituksen ja painikkeen komennon. Näkymä ei lähetä HTTP-pyyntöjä itse.
- **Palvelut** hoitavat erillisiä tehtäviä: `ShoplistApiService` kommunikoi palvelimen kanssa ja `ShellNavigationService` huolehtii sivujen välisestä navigoinnista.

### Mikä on tietojen sidonta?

Tietojen sidonta (*data binding*) yhdistää käyttöliittymän ominaisuuden näkymämallin ominaisuuteen. Esimerkiksi pääsivun `ItemsSource="{Binding Shoplist}"` näyttää näkymämallin `Shoplist`-kokoelman. Kun näkymämalli ilmoittaa ominaisuuden muuttuneen, käyttöliittymä voi päivittyä.

Pääsivulla `SelectedItem` käyttää kaksisuuntaista sidontaa (`TwoWay`): käyttäjän tekemä valinta välittyy näkymämalliin, ja näkymämalli voi puolestaan tyhjentää valinnan. Painikkeiden `Command`-sidonnat yhdistävät painikkeet näkymämallien komentoihin.

### Mitä rajapinnat ja riippuvuuksien injektointi tarkoittavat?

Rajapinta (*interface*) määrittelee, mitä toimintoja palvelu tarjoaa, mutta ei itse kerro, miten ne toteutetaan. Esimerkiksi `IShoplistService` määrittelee listan hakemisen, tuotteen lisäämisen ja keräämisen. `ShoplistApiService` toteuttaa nämä toiminnot HTTP-pyyntöinä.

Riippuvuuksien injektoinnissa (*dependency injection*, DI) luokka saa tarvitsemansa oliot rakentajansa parametreina sen sijaan, että se luo ne itse. Esimerkiksi `ShoplistViewModel` saa `IShoplistService`- ja `IAppNavigationService`-palvelut. `MauiProgram` rekisteröi rajapinnoille toteutukset. Tämän ansiosta näkymämallin toimintaa voi tarvittaessa testata myös korvaavilla palveluilla.

### Asynkroniset toiminnot

Palvelinkutsut ja navigointi ovat asynkronisia: niitä odotetaan `await`-avainsanalla. Näin käyttöliittymän ei tarvitse odottaa verkkovastausta estävästi. `Task` kuvaa toimintoa, joka voi valmistua myöhemmin; esimerkiksi `Task<IReadOnlyList<Shoplist>>` tuottaa valmistuessaan tuotelistan. Näkymämallit käyttävät lataus- ja varaustiloja estämään tiettyjä päällekkäisiä toimintoja.

## Projektin rakenne

Ratkaisun juuressa on `ShoppingMauiMvvm.slnx`. Varsinainen sovellus sijaitsee `ShoppingMauiMvvm`-kansiossa:

```text
ShoppingMauiMvvm
├── Models             Tuotteen tietomalli
├── ViewModels         Näkymien tila ja komennot
├── Services           Rajapinnat sekä HTTP- ja navigointipalvelut
├── Platforms          Alustakohtaiset käynnistys- ja asetustiedostot
├── Resources          Esimerkiksi värit, tyylit, fontit ja sovelluskuvake
├── App.xaml           Sovelluksen yhteiset resurssit
├── AppShell.xaml      Sovelluksen sivurakenne
├── MainPage.xaml      Ostoslistan näkymä
├── AddPage.xaml       Tuotteen lisäysnäkymä
├── MauiProgram.cs     Sovelluksen ja palveluiden alustaminen
└── ShoppingMauiMvvm.csproj   Projektin asetukset ja riippuvuudet
```

XAML-tiedostojen rinnalla on samannimisiä `.xaml.cs`-tiedostoja, jotka sisältävät näkymän tai sovelluksen C#-koodin. `bin`- ja `obj`-kansiot puolestaan sisältävät käännöksen tuottamia tiedostoja, eivät sovelluksen varsinaista käsin kirjoitettua lähdekoodia.

## Ajaminen ja toimintaympäristö

Projekti kohdistuu .NET 10:een ja käyttää .NET MAUI -työkuormaa. Projektitiedostossa on kohteet Androidille, iOS:lle ja Mac Catalystille sekä Windowsilla lisäksi Windowsille. Käytettävissä olevat kohteet ja niiden kääntäminen riippuvat kehityskoneesta ja asennetuista alusta- ja MAUI-työkaluista; Apple-kohteet tarvitsevat lisäksi Apple-kehitysympäristön.

1. Avaa `ShoppingMauiMvvm.slnx` Visual Studiossa, jossa .NET MAUI -kehitystyökalut on asennettu.
2. Valitse käytettävissä oleva kohdealusta ja laite tai emulaattori.
3. Käännä ja käynnistä sovellus Visual Studiosta.

Ostoslistatoiminnot tarvitsevat verkkoyhteyden ja toimivan taustapalvelun. Palvelimen perusosoite määritellään `MauiProgram.cs`-tiedostossa `HttpClient`-olion `BaseAddress`-ominaisuudessa. Jos osoite tai palvelin muuttuu, asiakassovelluksen asetus on päivitettävä vastaavasti.