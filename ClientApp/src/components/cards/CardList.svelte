<script lang="ts">
    import CardListEntry from "./CardListEntry.svelte";
    import {cards} from "$lib/state";
    
    function onRemoveButtonClicked(event: any) {
        const title = event.detail;
        console.debug(`Removing cards with title: ${title}`);
        cards.update(c => c.filter(x => x.title !== title));
    }
</script>

<div>
    <a href="/export">Export</a>
    {#each $cards as card}
        <CardListEntry card="{card}" on:removeButtonClicked={onRemoveButtonClicked}/>
    {/each}
</div>
<style>
    a {
        display: block;
        text-align: center;
        text-decoration: none;
        font-size: 2em;
        font-weight: bold;
        color: inherit;
    }
    div {
        border: thin solid black;
        border-radius: 5px;
        padding: 0 10px 10px 10px;
    }
</style>

