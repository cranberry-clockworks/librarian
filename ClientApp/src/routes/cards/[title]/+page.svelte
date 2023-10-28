<script lang="ts">
    import type {CardPageParameters} from "./+page";
    import {cards} from "$lib/state";
    import type {Card} from "$lib/models";
    import PronunciationButton from "../../../components/definitions/PronunciationButton.svelte";

    export let data: CardPageParameters;
    
    const card: Card | undefined = $cards.find(c => c.title == data.title);
</script>

<div class="container">
{#if typeof card !== "undefined"}
    <h1>{card.title}</h1>
    
    <h2>Front</h2>
    <p class="box">{@html card.front}</p>

    <h2>Back</h2>
    <p class="box">{@html card.back}</p>
    
    <h2>Media</h2>
    {#each Object.keys(card.media) as key (key)}
        <PronunciationButton title={key} fetch={() => Promise.resolve(card.media[key])}/>
    {/each}
{:else}
    <p>Not found</p>
{/if}
</div>

<style>
    .box {
        border: thin solid cornflowerblue;
        padding: 10px 20px;
        border-radius: 5px;
        margin-bottom: 10px;
        text-align: center;
    }
</style>