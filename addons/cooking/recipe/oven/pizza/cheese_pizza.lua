--- @author omnisudo
--- @date 3/9/2025

return require( "recipe.oven" )({
    Input = {
        ["cooking.item.ingredient.dough"] = 1,
        ["cooking.item.ingredient.tomato"] = 1,
        ["cooking.item.ingredient.cheese"] = 1,
    },
    Output = function( inputs ) 
        return {
            [ {
                ID = "cooking.item.food.pizza.cheese_pizza",
                --- TODO:
                --- Set quality of pizza based off of quality of 
                --- ingredients with chance to upgrade based off of cooking level
                Quality = nil
            } ] = 1
        }
    end,
    Duration = 1
})