<script lang="ts">
    import {page} from "$app/stores";
    import axios from "axios";

    async function search(word: string | null) {
        let client = axios.create({baseURL: '/api'});
        let response = await client.get(`/definition/${word}`);
        return response.data;
    }
</script>


<p>
    {#await search($page.url.searchParams.get("q"))}
        Loading...
    {:then word}
        The word is {word}
    {:catch error}
        There is an error: ${error}
    {/await}
</p>