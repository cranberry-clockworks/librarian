<script lang="ts">
    import CardListEntry from "./CardListEntry.svelte";
    import {cards} from "$lib/state";
    import {ExportService} from "$lib/generated/client";
    
    function onRemoveButtonClicked(event: any) {
        const title = event.detail;
        console.debug(`Removing cards with title: ${title}`);
        cards.update(c => c.filter(x => x.title !== title));
    }
    
    async function exportCards() {
        await ExportService.export({deck: "Norsk", cards: $cards});
    }
</script>

<h1>Export list</h1>
<button on:click={exportCards}>Export</button>
{#each $cards as card}
    <CardListEntry card="{card}" on:removeButtonClicked={onRemoveButtonClicked}/>
{/each}

