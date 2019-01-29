namespace PersianNews.Engine

open Newtonsoft.Json
open System.Collections.Generic

type NewsAgency =
    | AsrIran = 0
    | FarsNews =  1

type NewsItem = {
    NewsAgency : NewsAgency
    Category : string seq
    Title : string
    RuTitr : string
    Subtitle : string
    Body : string
    Tags : string seq
}

type CleansedItem = {
    NewsAgency : NewsAgency
    Category : string seq
    Title : string seq
    RuTitr : string seq
    Subtitle : string seq
    Body : string seq
    Tags : string seq
}

type AsrIranCategories =
    | Ejtemaee = 0
    | Siasi = 1
    | Varzeshi = 2
    | Beinolmelal = 3
    | FarhangiHonari = 4
    | Eghtesadi = 5
    | SiasatKhareji = 6
    | KhandaniVaDidani = 7
    | Omoomi = 8
    | Salamat = 9
    | Elmi = 10
    | Ravanshenasi = 11
    | Havades = 12
    | Sargarmi = 13
    | IT = 14
    | DastanKootah = 15
    | Download = 16
    | Ashpazi = 17
    | Payamak = 18
    | SafarOTafrih = 19
    | UNKNOWN = 20

type FormalDocument = FormalDocument of (string seq * NewsAgency)
type AsrDocument = Document of (string seq * AsrIranCategories)
