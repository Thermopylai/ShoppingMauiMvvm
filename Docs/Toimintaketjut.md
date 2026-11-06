# Sovelluksen toimintaketjut

[Takaisin yleiskuvaan](README.md) | [Luokat ja tiedostot](Luokat.md)

Seuraavat esimerkit näyttävät, miten vastuut jakautuvat käyttöliittymän, näkymämallin ja palveluiden välillä. Sovelluksen käyttöliittymätekstit ovat tällä hetkellä englanninkielisiä.

## 1. Sovelluksen käynnistyminen

1. Käyttöjärjestelmä käynnistää oman alustakohtaisen sovellusluokkansa.
2. Alustakohtainen luokka kutsuu `MauiProgram.CreateMauiApp`-metodia, joka rakentaa sovelluksen ja rekisteröi sen riippuvuudet.
3. Yhteinen `App` luo ikkunan, jonka sisältönä on `AppShell`.
4. `AppShell` määrittelee pääsivun ja lisäyssivun reitit. Pääsivun näkymä käyttää `ShoplistViewModel`-oliota.
5. Kun `MainPage` tulee näkyviin, se tarkistaa `LoadCommand`-komennon suoritettavuuden ja käynnistää latauksen, jos se on mahdollista.

## 2. Ostoslistan lataaminen ja päivittäminen

```text
MainPage.OnAppearing tai RefreshView
    -> ShoplistViewModel.LoadCommand
    -> LoadAsync
    -> IShoplistService.GetAllAsync
    -> ShoplistApiService: GET api/shoplist
    -> tuotteet Shoplist-kokoelmaan
    -> CollectionView näyttää tuotteet
```

1. Komento aloittaa latauksen asettamalla `IsLoading`-arvon todeksi ja tyhjentämällä vanhan virheilmoituksen.
2. Palvelu hakee tuotteet taustapalvelusta ja muuntaa JSON-vastauksen `Shoplist`-olioiden listaksi.
3. Onnistuneen haun jälkeen näkymämalli tyhjentää mahdollisen valinnan ja muodostaa tuotteista uuden `ObservableCollection`-kokoelman.
4. Sidonta välittää uuden kokoelman pääsivun `CollectionView`-ohjaimelle.
5. Jos haku epäonnistuu, näkymämalli asettaa `ErrorMessage`-arvon. Vanha näkyvä lista jää paikalleen. Lopuksi `IsLoading` palautetaan epätodeksi myös virhetilanteessa.

Käyttäjän käynnistämä päivitys käyttää samaa `LoadCommand`-komentoa kuin sivun ilmestyessä tehtävä lataus. `RefreshView` sitoo päivityksen näkyvän tilan `IsLoading`-ominaisuuteen yksisuuntaisesti.

## 3. Tuotteen lisääminen

1. Käyttäjä painaa pääsivun lisäyspainiketta. `OpenAddItemCommand` pyytää navigointipalvelua avaamaan `AddPage`-sivun.
2. Käyttäjä syöttää nimen ja määrän. Kenttien sidonnat välittävät tekstit `AddItemViewModel`-olion ominaisuuksiin `NewItemName` ja `NewItemQuantity`.
3. Lisäyspainike suorittaa `AddItemCommand`-komennon. Näkymämalli tarkistaa ensin määrän ja nimen. Virheellinen syöte näytetään käyttäjälle, eikä palvelupyyntöä lähetetä.
4. Kelvollisista tiedoista luodaan `Shoplist`-olio, jonka nimi on siistitty `Trim`-metodilla. `ShoplistApiService` lähettää olion JSON-muodossa `POST api/shoplist` -pyynnöllä.
5. Jos tallennus onnistuu, näkymämalli tyhjentää kentät ja pyytää navigointipalvelua siirtymään reitille `//MainPage`. Pääsivun tullessa näkyviin se yrittää ladata listan uudelleen.
6. Jos lisäyspyyntö epäonnistuu, käyttäjä saa lisäysvirheilmoituksen. Jos tallennus onnistuu mutta navigointi epäonnistuu, ilmoitus kertoo erikseen, että tuote lisättiin. Kenttien tyhjentäminen auttaa välttämään saman tuotteen tahattoman uudelleenlähetyksen.

Lisäyksen aikana `IsBusy` näyttää toimintaindikaattorin ja estää lisäyskomennon suorittamisen uudelleen.

## 4. Tuotteen kerääminen eli poistaminen

1. Käyttäjä valitsee tuotteen pääsivun listalta. Kaksisuuntainen sidonta päivittää `SelectedItem`-arvon.
2. `CanCollectSelectedItem` sallii keräämisen, kun tuote on valittu eikä listan lataus tai kerääminen ole kesken.
3. Käyttäjä painaa **Collect selected item**. `CollectSelectedItemCommand` välittää valitun tuotteen `Id`-arvon palvelun `CollectAsync`-metodille.
4. `ShoplistApiService` lähettää `DELETE api/shoplist/{itemId}` -pyynnön. Onnistuneen vastauksen jälkeen näkymämalli tyhjentää valinnan ja poistaa tuotteen näkyvästä kokoelmasta.
5. Näkymämalli yrittää hakea listan uudelleen, jotta se vastaisi palvelimen tilaa. Jos tämä haku epäonnistuu, käyttäjälle kerrotaan, että tuote kerättiin mutta päivitys epäonnistui; kerättyä tuotetta ei palauteta vanhaan näkyvään kokoelmaan.
6. Jos varsinainen poistopyyntö epäonnistuu, näkymämalli näyttää poistovirheen eikä poista tuotetta itse näkyvästä kokoelmasta. Molemmissa tapauksissa lataustila palautetaan lopuksi.

## Virheiden käsittelyn rajat

HTTP-virheet, JSON-vastauksen käsittelyvirheet ja navigoinnin poikkeukset välittyvät näkymämalleille. Ne muodostavat käyttäjälle näkyvän virheilmoituksen `ErrorMessage`-ominaisuuteen. Virheilmoituksessa näytetään myös poikkeuksen viesti. Asynkronisen toiminnon `finally`-lohko palauttaa lataus- tai varaustilan, jotta käyttöliittymä ei jää pysyvästi odotustilaan kyseisen poikkeuksen vuoksi.

Verkkopyynnön epäonnistuessa asiakas ei aina voi tietää, suorittiko palvelin pyynnön ennen yhteysvirhettä. Etenkin lisäämisen tai poistamisen jälkeen lista kannattaa tarvittaessa päivittää ennen uutta yritystä.