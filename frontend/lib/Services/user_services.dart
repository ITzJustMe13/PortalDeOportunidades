
import 'package:frontend/Models/User.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:flutter/widgets.dart';
import 'package:provider/provider.dart';

class UserServices {
  /// Documentation for getCachedUser
/// @param: BuildContext context
/// Function used to get the current logged in User
/// @returns: User? Dto
 static Future<User?> getCachedUser(BuildContext context) async {
    try {
      final user = await Provider.of<UserApiHandler>(context, listen: false)
          .getStoredUser();
      return user;
    } catch (e) {
      print('Error fetching user: $e');
      return null;
    }
  }
}
