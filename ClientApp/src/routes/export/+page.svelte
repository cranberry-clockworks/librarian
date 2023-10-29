<script lang="ts">
    import {AnkiService} from "$lib/generated/client";
</script>

<h1>Export</h1>

<!--{#await get()}-->
<!--    <p>Loading available Anki decks...</p>-->
<!--{:then decks}-->
<!--&lt;!&ndash;    <form>&ndash;&gt;-->
<!--        {#each decks as deck}-->
<!--            <div>{deck}</div>-->
<!--&lt;!&ndash;            <input type="radio" value="{deck}">&ndash;&gt;-->
<!--        {/each}-->
<!--&lt;!&ndash;        <input type="button" value="Export">&ndash;&gt;-->
<!--&lt;!&ndash;    </form>&ndash;&gt;-->
<!--{:catch error}-->
<!--    There is an error: ${error}-->
<!--{/await}-->

{#await AnkiService.getDecks()}
    <!-- Display a loading indicator while data is being fetched -->
    <p>Loading...</p>
{:then data}
    <!-- Display the loaded data -->
    <ul>
        {#each data as item (item)}
            <li>{item}</li>
        {/each}
    </ul>
{:catch error}
    <!-- Handle error if the promise rejects -->
    <p>Error: {error.message}</p>
{/await}