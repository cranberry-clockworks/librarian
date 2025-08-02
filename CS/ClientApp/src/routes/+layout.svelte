<script>
    import CardList from "../components/cards/CardList.svelte";
    import SearchForm from "../components/SearchForm.svelte";
</script>

<div class="row">
    <div class="column left">
        <CardList/>
    </div>
    
    <div class="column right">
        <nav>
            <SearchForm/>
        </nav>
        <slot/>
    </div>
</div>


<style>
    
    
    .row {
        display: flex;
    }
    .column {
        float: left;
        margin-left: 10px;
    }
    .left {
        width: 25%;
        
    }
    .right {
        width: 75%;
    }
    .row:after {
        content: "";
        display: table;
        clear: both;
    }
</style>

