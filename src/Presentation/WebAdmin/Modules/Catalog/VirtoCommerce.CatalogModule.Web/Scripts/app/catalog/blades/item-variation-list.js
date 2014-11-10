﻿angular.module('catalogModule.blades.itemVariationList', [
])
.controller('itemVariationListController', ['$scope', 'bladeNavigationService', 'dialogService', 'items', function ($scope, bladeNavigationService, dialogService, items) {

    $scope.blade.refresh = function (parentRefresh) {
        items.get({ id: $scope.blade.itemId }, function (data) {
            $scope.blade.item = data;
            $scope.blade.isLoading = false;
        });
    }

    $scope.selectVariation = function (listItem) {
        $scope.selectedItem = listItem;

        var blade = {
            id: 'variationDetail',
            itemId: listItem.id,
            title: listItem.code,
            style: 'gray',
            subtitle: 'Variation details',
            controller: 'itemDetailController',
            template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/item-detail.tpl.html'
        };
        bladeNavigationService.showBlade(blade, $scope.blade);
    };

    function closeChildrenBlades() {
        angular.forEach($scope.blade.childrenBlades.slice(), function (child) {
            bladeNavigationService.closeBlade(child);
        });
    }

    $scope.bladeToolbarCommands = [
        {
            name: "Refresh", icon: 'icon-spin',
            executeMethod: function () {
                $scope.blade.refresh();
            },
            canExecuteMethod: function () {
                return true;
            }
        },
      {
          name: "Delete", icon: 'icon-remove',
          executeMethod: function () {
              var dialog = {
                  id: "confirmDeleteItem",
                  title: "Delete confirmation",
                  message: "Are you sure you want to delete selected Variations?",
                  callback: function (remove) {
                      if (remove) {
                          closeChildrenBlades();

                          var ids = [];
                          angular.forEach($scope.blade.item.variations, function (variation) {
                              if (variation.selected)
                                  ids.push(variation.id);
                          });

                          items.remove({}, ids, function () {
                              $scope.blade.refresh();
                          });
                      }
                  }
              }

              dialogService.showConfirmationDialog(dialog);
          },
          canExecuteMethod: function () {
              var retVal = false;
              if (angular.isDefined($scope.blade.item)) {
                  retVal = _.any($scope.blade.item.variations, function (x) { return x.selected; });
              }
              return retVal;
          }
      }
    ];

    $scope.checkAll = function (selected) {
        angular.forEach($scope.blade.item.variations, function (variation) {
            variation.selected = selected;
        });
    };


    $scope.blade.refresh();
}]);

