<script lang="ts">
    import {PronunciationsService, TranslationsService} from "$lib/generated/client/index.js";
    import PronunciationButton from "./PronunciationButton.svelte";
    import {createCard, getNameForMedia} from "$lib/models";
    import {cards} from "$lib/state";

    export let phrase = "en Lemma";
    export let wordClass = "Unknown";
    
    let translation = ""
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
        if (translation.length === 0) {
            await fetchTranslation();
        }
        
        const media: Record<string, string> = {};
        const mainAudio = getNameForMedia(phrase);
        media[mainAudio] = await fetchAudio();
        
        const card = createCard(wordClass, phrase, translation, media, "");
        
        if ($cards.find(c => c.title === card.title)) {
            return;
        }
        cards.update(c => [...c, card]);
    }
</script>
<div style="margin-bottom: 10px">
    <span class="lemma">{phrase}</span>
    <span class="wordClass">{wordClass}</span>
</div>
<div>
    <button on:click={AddCard}>Add</button>
    <PronunciationButton fetch="{fetchAudio}"/>
    {#if translation.length > 0}
        <span>{translation}</span>
    {:else}
        <button on:click={fetchTranslation}>Translate</button>
    {/if}
    <slot/>
</div>

<style>
    .lemma {
        font-size: xx-large;
        margin-right: 10px;
    }
    .wordClass {
        text-transform: uppercase;
        margin-right: 10px;
    }
</style>