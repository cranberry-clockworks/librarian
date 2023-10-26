<script lang="ts">
    import CardListEntry from "./CardListEntry.svelte";
    import {cards} from "$lib/state";
    
    function onRemoveButtonClicked(event: any)
    {
        const title = event.detail;
        
        console.debug(`Removing cards with title: ${title}`);
        
        cards.update(c => c.filter(x => x.title !== title));
    }
</script>

<h1>Export list</h1>
{#each $cards as card}
    <CardListEntry card="{card}" on:removeButtonClicked={onRemoveButtonClicked}/>
{/each}

