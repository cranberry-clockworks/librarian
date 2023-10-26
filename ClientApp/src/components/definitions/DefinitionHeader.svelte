<script lang="ts">
    import {PronunciationsService, TranslationsService} from "$lib/generated/client/index.js";
    import PronunciationButton from "./PronunciationButton.svelte";

    export let phrase = "en Lemma";
    export let wordClass = "Unknown";
    
    let translation = ""
    let audio = ""
    
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
</script>
<div style="margin-bottom: 10px">
    <span class="lemma">{phrase}</span>
    <span class="wordClass">{wordClass}</span>
</div>
<div>
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