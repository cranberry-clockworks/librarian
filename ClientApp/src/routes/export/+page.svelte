<script lang="ts">
    import {AnkiService} from "$lib/generated/client";
    import {cards} from "$lib/state";
    
    let selectedDeck = "Default";
    
    async function send() {
        await AnkiService.export({deck: selectedDeck, cards: $cards});
        cards.update(c => []);
    }
</script>

<h1>Export</h1>

{#if $cards.length > 0}
    {#await AnkiService.getDecks()}
        <p>Loading available decks...</p>
    {:then decks}
        <form on:submit={send}>
            <h2>Choose the deck:</h2>
            <p>
            {#each decks as deck}
                <input type="radio" name="deck" id="{deck}" value="{deck}" bind:group={selectedDeck}>
                <label for="{deck}">
                    {deck}
                </label>
                <br/>
            {/each}
            <br/>
            <button type="submit">Export</button>
            </p>
        </form>
    {:catch error}
        <p>
            There is an error: ${error}
        </p>
    {/await}
{:else }
    <p>
        Nothing to export
    </p>
{/if}