﻿angular.module('virtoCommerce.customerModule.blades')
.controller('customerPhonesListController', ['$scope', 'bladeNavigationService', 'dialogService', function ($scope, bladeNavigationService, dialogService) {
    $scope.selectedItem = null;

    function transformDataElement(data) {
        return { value: data };
    }

    function initializeBlade(data) {
        // transfornation to make complex object. Simple string isn't editable.
        data = _.map(data, transformDataElement);

        $scope.blade.currentEntities = angular.copy(data);
        $scope.blade.origEntity = data;
        $scope.blade.isLoading = false;
    };

    $scope.selectItem = function (listItem) {
        $scope.selectedItem = listItem;
    };

    $scope.blade.onClose = function (closeCallback) {
        if (isDirty()) {
            var dialog = {
                id: "confirmItemChange",
                title: "Save changes",
                message: "The Phone number has been modified. Do you want to save changes?"
            };
            dialog.callback = function (needSave) {
                if (needSave) {
                    $scope.saveChanges();
                }
                closeCallback();
            };
            dialogService.showConfirmationDialog(dialog);
        }
        else {
            closeCallback();
        }
    };

    function isDirty() {
        return !angular.equals($scope.blade.currentEntities, $scope.blade.origEntity);
    };

    $scope.cancelChanges = function () {
        $scope.bladeClose();
    }

    $scope.saveChanges = function () {
        var values = _.pluck($scope.blade.currentEntities, 'value');
        angular.copy(values, $scope.blade.data);
        angular.copy($scope.blade.currentEntities, $scope.blade.origEntity);
        $scope.bladeClose();
    };

    $scope.bladeToolbarCommands = [
        {
            name: "Add", icon: 'fa fa-plus',
            executeMethod: function () {
                $scope.blade.currentEntities.push(transformDataElement(''));
            },
            canExecuteMethod: function () {
                return true;
            }
        },
        {
            name: "Reset", icon: 'fa fa-undo',
            executeMethod: function () {
                angular.copy($scope.blade.origEntity, $scope.blade.currentEntities);
            },
            canExecuteMethod: function () {
                return isDirty();
            }
        },
        {
            name: "Delete", icon: 'fa fa-trash-o',
            executeMethod: function () {
                var idx = $scope.blade.currentEntities.indexOf($scope.selectedItem);
                if (idx >= 0) {
                    $scope.blade.currentEntities.splice(idx, 1);
                }
            },
            canExecuteMethod: function () {
                return $scope.selectedItem;
            }
        }
    ];

    $scope.$watch('blade.parentBlade.currentEntity.phones', function (currentEntities) {
        $scope.blade.data = currentEntities;
        initializeBlade($scope.blade.data);
    });

    // on load: 
    // $scope.$watch('blade.parentBlade.currentEntity.phones' gets fired
}]);