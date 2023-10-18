<script lang="ts">
    import {PronunciationsService, TranslationsService} from "$lib/generated/client/index.js";
    import AudioPlayer from "./AudioPlayer.svelte";

    export let article = ""
    export let lemma = "Lemma"
    export let wordClass = "Unknown"
    export let translation = ""
    export let audio = ""

    async function translate(word: string) {
        return TranslationsService.translate(word);
    }

    function buildPhrase()
    {
        return (article.length > 0 ? article + " " : "") + lemma;
    }
    
    async function fetchAudio() : Promise<string>
    {
        if (audio.length > 0)
            return audio;
        
        audio = await PronunciationsService.pronounce(buildPhrase());
        return audio;
    }
</script>

<div style="margin-bottom: 10px">
    <span class="lemma">{buildPhrase()}</span>
    <span class="wordClass">{wordClass}</span>
</div>
<div>
    <AudioPlayer fetch="{fetchAudio}"/>
    {#if translation.length === 0}
        <button on:click={async () => translation = await translate(lemma)}>Translate</button>
    {:else}
        <span>{translation}</span>
    {/if}
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