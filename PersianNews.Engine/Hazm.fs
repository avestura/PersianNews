namespace PersianNews.Engine

open NHazm

type Hazm() =
    static member WordTokenizer = WordTokenizer ()
    static member Normalizer = Normalizer ()
    static member Stemmer = Stemmer ()
    static member Lemmatizer = Lemmatizer ()
    static member POSTagger = POSTagger ()
    static member SentenceTokenizer = SentenceTokenizer ()