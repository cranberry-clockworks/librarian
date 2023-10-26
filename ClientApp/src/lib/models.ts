export type Card = {
    title: string,
    front: string,
    back: string,
    media: Record<string, string>
}

export function createCard(
    wordClass: string,
    phrase: string,
    translation: string,
    media: Record<string, string>,
    extra: string) : Card
{
    const title = `${phrase} (${wordClass.toLowerCase()})`; 
    const front = `<i>${wordClass}</i><br/><br/><b>${phrase}</b><br/>[sound:${getNameForMedia(phrase)}]<br/>${extra}`;
    const back = `<i>${wordClass}</i><br/><br/><b>${translation}</b>`
    
    return {
        title,
        front,
        back,
        media
    };
}

export function getNameForMedia(phrase: string) {
    return `no.${phrase}.mp3`;
}