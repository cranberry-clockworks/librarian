<script lang="ts">
    import CardListEntry from "./CardListEntry.svelte";
    import {cards} from "$lib/state";
    
    function onRemoveButtonClicked(event: any) {
        const title = event.detail;
        console.debug(`Removing cards with title: ${title}`);
        cards.update(c => c.filter(x => x.title !== title));
    }
    
    function onRemoveAllButtonClicked() {
        cards.update(c => [])    
    }
</script>

<div class="main">
    <div style="margin-bottom: 10px">
        <a href="/export">Export</a>
    </div>
    <div>
        <button on:click={onRemoveAllButtonClicked}>Remove all</button>
    </div>
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
    .main {
        border: thin solid black;
        border-radius: 5px;
        padding: 0 10px 10px 10px;
    }
</style>

