import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Models/Review.dart';
import 'package:frontend/Views/HistoryReservationScreen.dart';
import 'package:frontend/Views/ReviewsHistoryScreen.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Services/review_api_handler.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/DynamicTextField.dart';
import 'package:flutter/services.dart';

class CreateReviewScreen extends StatefulWidget {
  final Reservation reservation;

  const CreateReviewScreen({super.key, required this.reservation});

  @override
  _CreateReviewScreenState createState() => _CreateReviewScreenState();
}
  
class _CreateReviewScreenState extends State<CreateReviewScreen> {
  late ScrollController verticalScrollController;
  late TextEditingController descriptionController;
  late TextEditingController ratingController;
  bool reviewExists = false;
  Review? review;

  @override
  void initState() {
    super.initState();
    verticalScrollController = ScrollController();
    descriptionController = TextEditingController();
    ratingController = TextEditingController();
    _initializeReview();
  }

  @override
  void dispose() {
    verticalScrollController.dispose();
    descriptionController.dispose();
    ratingController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: CustomAppBar(),
        endDrawer: CustomDrawer(),
        body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            return _buildTabletLayout();
          } else {
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return SingleChildScrollView(
      controller: verticalScrollController,
      padding: const EdgeInsets.all(20.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Dê a sua Opinião:",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          DynamicTextField(
            label: 'Descrição',
            controller: descriptionController,
            maxLines: 1,
          ),
          SizedBox(height: 20),
          DynamicTextField(
            label: 'Em quanto avalia a sua Oportundade? (0-5)',
            controller: ratingController,
            maxLines: 1,
            inputType: TextInputType.number,
            inputFormatters: [
              FilteringTextInputFormatter.allow(RegExp(r'^\d*\.?\d{0,2}$')),
            ],
          ),
          SizedBox(height: 20),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Color(0xFF50C878),
            ),
            onPressed: () {
              _saveReview();
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => ReviewsHistoryScreen()
                ),
              );
            },
            child: Text(
              'Guardar',
              style: TextStyle(color: Colors.white), // Adjust text color if needed
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTabletLayout() {
    return _buildMobileLayout();
  }

  Widget _buildDesktopLayout() {
    return _buildMobileLayout();
  }

  Future<Review?> getCreatedReview(Reservation reservation) async {
    if (reservation.reservationId != null) {
      try {
        final existingReview = await Provider.of<ReviewApiHandler>(context, listen: false)
            .getReviewById(reservation.reservationId!);

        if (existingReview != null) {
          descriptionController.text = existingReview.description ?? '';
          ratingController.text = existingReview.rating.toString();
          return existingReview;
        }
      } catch (e) {
        print('Error fetching review: $e');
      }
    }
    return null;
  }

  Future<void> _saveReview() async {
    final String ratingText = ratingController.text.replaceAll(',', '.');

    if (ratingText.isEmpty) {
      _showSnackBar('Por favor, insira uma classificação válida entre 0 e 5.', Colors.red);
      return;
    }

    final double? rating = double.tryParse(ratingText);
    if (rating == null || rating < 0 || rating > 5) {
      _showSnackBar('Por favor, insira uma classificação válida entre 0 e 5.', Colors.red);
      return;
    }

    final Review reviewToSave = Review(
      reservationId: widget.reservation.reservationId!,
      rating: rating,
      description: descriptionController.text.isNotEmpty ? descriptionController.text : null,
    );

    bool success = false;

    try {
      if (reviewExists) {
        // Update existing review


        success = await Provider.of<ReviewApiHandler>(context, listen: false).editReview(
          widget.reservation.reservationId!,
          reviewToSave,
        );
      } else {
        // Create a new review
        final createdReview = await Provider.of<ReviewApiHandler>(context, listen: false)
            .createReview(reviewToSave);
        success = createdReview != null;
      }
    } catch (e) {
      success = false;
    }

    if (success) {
      _showSnackBar('Opinião guardada com sucesso!', Colors.green);
      _navigateToHistory();
    } else {
      _showSnackBar('Falha ao guardar a opinião. Tente novamente!', Colors.red);
    }
  }

  void _showSnackBar(String message, Color color) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: color,
      ),
    );
  }

  void _navigateToHistory() {
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(
        builder: (context) => HistoryReservationScreen(),
      ),
    );
  }

  Future<void> _initializeReview() async {
    final existingReview = await getCreatedReview(widget.reservation);
    if (existingReview != null) {
      setState(() {
        review = existingReview;
        reviewExists = true;
      });
    } else {
      setState(() {
        reviewExists = false;
      });
    }
  }

  
}