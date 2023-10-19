<script lang="ts">
    import DefinitionHeader from "./DefinitionHeader.svelte";
    import type {VerbDefinition} from "$lib/generated/client";

    export let definition: VerbDefinition
    
    function strip(type: string) {
        return type.replaceAll("<", "").replaceAll(">", "")
    }
</script>

<DefinitionHeader lemma={definition.lemma} wordClass="verb"/>

<table>
    <tr>
        <th>Inflection</th>
        <th>Word</th>
    </tr>
    {#each definition.inflections ?? [] as inflection}
        <tr>
            <td>{strip(inflection.type ?? "")}</td>
            <td>{inflection.word}</td>
        </tr>
    {/each}
</table>

<style>
    table {
        margin-top: 20px;
        border-collapse: collapse;
        width: 100%;
        margin-bottom: 20px;
        border: 1px solid #ddd;
    }
    
    th {
        background-color: #f2f2f2;
        color: #333;
        font-weight: bold;
        font-family: sans-serif;
        padding: 10px;
        text-align: left;
        border: 1px solid #ddd;
    }
    
    td {
        padding: 10px;
        border: 1px solid #ddd;
    }
    
    tr:hover {
        background-color: #f5f5f5;
    }

</style>