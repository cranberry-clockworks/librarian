<script lang="ts">
    import {PronunciationsService, TranslationsService} from "$lib/generated/client/index.js";
    import PronunciationButton from "./PronunciationButton.svelte";
    import {createCard, getNameForMedia} from "$lib/models";
    import {cards} from "$lib/state";

    export let isSub = false;
    
    export let phrase = "en Lemma";
    export let wordClass = "Unknown";
    
    let translation : undefined | string
    let audio = ""
    
    let frontDefault = "";
    let frontAdditional = "";
    
    async function fetchAudio() : Promise<string> {
        if (audio.length > 0) {
            return audio;
        }
        
        audio = await PronunciationsService.pronounce(phrase);
        return audio;
    }
    
    async function fetchTranslation() {
        translation = await TranslationsService.translate(phrase);
    }
    
    async function AddCard() {
        if (typeof translation === "undefined") {
            await fetchTranslation();
        }
        
        const media: Record<string, string> = {};
        const mainAudio = getNameForMedia(phrase);
        media[mainAudio] = await fetchAudio();
        
        const card = createCard(wordClass, phrase, translation!, media, "");
        
        if ($cards.find(c => c.title === card.title)) {
            return;
        }
        cards.update(c => [...c, card]);
    }
</script>
<div style="margin-bottom: 10px">
    <span class="lemma {isSub ? 'sub': ''}">{phrase}</span>
    <span class="wordClass {isSub ? 'sub' : ''}">{wordClass}</span>
</div>
<div>
    <button on:click={AddCard}>Add</button>
    <PronunciationButton fetch="{fetchAudio}"/>
    {#if typeof translation != "undefined"}
        <span><input type="text" bind:value={translation}></span>
    {:else}
        <button on:click={fetchTranslation}>Translate</button>
    {/if}
</div>

<style>
    .lemma {
        font-size: 2em;
        margin-right: 10px;
    }
    .lemma.sub {
        font-size: 1.5em;
    }
    .wordClass {
        text-transform: uppercase;
        margin-right: 10px;
    }

    .wordClass.sub {
        font-size: 0.8em;
    }
    input[type="text"] {
        border: none;
        background: none;
        padding: 0;
        margin: 0;
        outline: none;
        font-size: inherit;
        font-family: inherit;
        color: inherit;
    }
</style>