import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/user_api_handler.dart';


class CreateOpportunityState with ChangeNotifier {
  var _userApiHandler = UserApiHandler();
  var _opportunityApiHandler = OpportunityApiHandler();

  String? _errorMessage;
  bool _isLoading = false;

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  CreateOpportunityState({
    UserApiHandler? userApiHandler,
    OpportunityApiHandler? opportunityApiHandler,
  })  : _userApiHandler = userApiHandler ?? UserApiHandler(),
        _opportunityApiHandler = opportunityApiHandler ?? OpportunityApiHandler();

// Setters para injeção das dependências nos testes
  set userApiHandler(UserApiHandler handler) {
    _userApiHandler = handler;
  }

  set opportunityApiHandler(OpportunityApiHandler handler) {
    _opportunityApiHandler = handler;
  }


  Future<int> getCurrentUserId() async {
    return await _userApiHandler.getStoredUserID();
  }

  // Function to register
  Future<Opportunity?> createOpportunity(
      Opportunity opportunity, List<OpportunityImg> opportunityImgs) async {
    _isLoading = true;
    notifyListeners();

    Opportunity? createdOpportunity =
        await _opportunityApiHandler.createOpportunity(opportunity);

    _isLoading = false;

    if (createdOpportunity == null) {
      _errorMessage = 'Ocorreu um erro ao criar Oportunidade';
      notifyListeners();

      return null;
    } else {
      for (OpportunityImg image in opportunityImgs) {
        createdOpportunity.opportunityImgs.add(OpportunityImg(
            imgId: 0,
            opportunityId: createdOpportunity.opportunityId,
            imageBase64: image.imageBase64));
      }
      bool isEditValid = await _opportunityApiHandler.editOpportunity(
          createdOpportunity.opportunityId, createdOpportunity);

      if (!isEditValid) {
        _errorMessage = 'Ocorreu um erro ao criar Oportunidade';
        notifyListeners();
        return null;
      }
    }

    notifyListeners();
    return createdOpportunity;
  }
}
