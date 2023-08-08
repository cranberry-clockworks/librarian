export type UnknownPartOfSpeech = {
    readonly root: string
}

export type Substantiv = {
    readonly root: string,
    readonly gender: "utrum" | "neutrum"
}

export type Verb = {
    readonly infinitive: string,
    readonly presens: string,
    readonly preteritum: string,
    readonly supinum: string,
    readonly imperative: string
}