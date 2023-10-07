<script lang="ts">
    import type {UnknownDefinition} from "$lib/generated/client";
    import DefinitionTitle from "./DefinitionTitle.svelte";

    export let definition: UnknownDefinition
</script>

<DefinitionTitle lemma={definition.lemma} wordClass="unknown"/>
