# ShoppingMauiMvvm

ShoppingMauiMvvm on .NET MAUI -ostoslistasovellus. Sillä voi tarkastella ja päivittää ostoslistaa, lisätä tuotteita sekä poistaa kerättyjä tuotteita. Sovellus on HTTP-rajapintaa käyttävä asiakas: taustapalvelimen ja tietokannan toteutukset eivät sisälly tähän repositorioon.

Sovelluksen käyttöliittymä on tällä hetkellä englanninkielinen.

## Ominaisuudet

- Ostoslistan lataaminen ja päivittäminen palvelimelta
- Tuotteen lisääminen nimellä ja positiivisella kokonaislukumäärällä
- Valitun tuotteen kerääminen eli poistaminen listalta
- Lataustilan ja mahdollisten virheiden näyttäminen käyttöliittymässä

## Teknologiat ja rakenne

Sovellus on toteutettu C#:lla ja XAML:lla .NET 10:n ja .NET MAUI:n avulla. Se käyttää MVVM-mallia: `Shoplist` on tietomalli, sivut muodostavat näkymät ja näkymämallit huolehtivat näkymien tilasta ja komennoista. Erilliset palvelut käsittelevät HTTP-pyynnöt ja navigoinnin. Riippuvuudet rekisteröidään tiedostossa `MauiProgram.cs`.

Sovellusprojekti on `ShoppingMauiMvvm`-kansiossa ja ratkaisu on `ShoppingMauiMvvm.slnx`. Projektissa on kohteet Androidille, iOS:lle ja Mac Catalystille sekä Windowsilla lisäksi Windowsille. Käytettävissä olevat kohteet riippuvat kehitysympäristöstä; Apple-kohteet edellyttävät Apple-kehitysympäristöä.

## Käyttöönotto

1. Asenna Visual Studioon .NET MAUI -kehitystyökalut ja tarvittavat kohdealustan työkalut.
2. Avaa `ShoppingMauiMvvm.slnx` Visual Studiossa.
3. Varmista, että sovelluksen käyttämä taustapalvelu on käynnissä ja tavoitettavissa valitusta laitteesta tai emulaattorista. Palvelimen perusosoite määritellään [MauiProgram.cs-tiedostossa](ShoppingMauiMvvm/MauiProgram.cs) `HttpClient`-olion `BaseAddress`-ominaisuudessa. Muuta osoitetta tarvittaessa.
4. Valitse käytettävissä oleva kohdealusta ja laite tai emulaattori, käännä projekti ja käynnistä sovellus.

Ostoslistatoiminnot edellyttävät verkkoyhteyttä ja yhteensopivaa taustapalvelua. Asiakas käyttää seuraavia rajapintakutsuja:

| Pyyntö | Tarkoitus |
| --- | --- |
| `GET api/shoplist` | Hakee tuotteet. |
| `POST api/shoplist` | Lisää tuotteen JSON-muodossa. |
| `DELETE api/shoplist/{itemId}` | Poistaa tuotteen tunnisteen perusteella. |

Tuotteen tietomallissa on tunniste `Id`, nimi `Item` ja määrä `Amount`. Palvelimen mahdolliset asennus- ja käynnistysohjeet eivät sisälly tähän projektiin.

## Dokumentaatio

- [Dokumentaation yleiskuva](Docs/README.md) – arkkitehtuuri, käsitteet ja projektin rakenne
- [Luokat ja tiedostot](Docs/Luokat.md) – sovellusluokkien ja palveluiden vastuut
- [Toimintaketjut](Docs/Toimintaketjut.md) – käynnistys sekä listan lataaminen, lisääminen ja poistaminen
